using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenAnimation;
using FrostbiteKitchen.KitchenStation;

public class Stove : MonoBehaviour, IInteractable, IPointerClickHandler
{
    [SerializeField] private KitchenStationAnimator stationAnimator;
    [SerializeField] private IngredientData currentIngredientOnStove;
    [SerializeField] private bool isCooking;
    [SerializeField] private bool isBurned;
    [SerializeField] private float burnTime = 11f;

    private IngredientData rawIngredientOnStove;
    private float overcookTimer;
    private bool isUnderThreat;
    private Coroutine cookingCoroutine;
    private int lastInteractFrame = -1;

    public bool IsCooking => isCooking;
    public bool HasIngredientOnStove => currentIngredientOnStove != null;

    public event System.Action<float> OnProgressUpdated;
    public event System.Action OnDishBurned;
    public event System.Action OnCookingFinished;
    public event System.Action OnCookingStarted;

    private void Awake()
    {
        if (stationAnimator == null)
            stationAnimator = GetComponent<KitchenStationAnimator>();
    }

    private void Start()
    {
        GameStateMachine.OnStateChanged += HandleGameStateChanged;
        OnProgressUpdated?.Invoke(0f);
    }

    private void OnDestroy()
    {
        GameStateMachine.OnStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (!isCooking && currentIngredientOnStove != null && isUnderThreat && !isBurned)
            HandleOvercooking();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Interact();
    }

    public void Interact()
    {
        if (Time.frameCount == lastInteractFrame)
            return;

        lastInteractFrame = Time.frameCount;

        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
            return;

        if (isBurned)
        {
            ResetStove();
            return;
        }

        if (currentIngredientOnStove != null)
        {
            TryTakeIngredientFromStove(inventory);
            return;
        }

        TryPlaceHeldIngredientOnStove(inventory);
    }

    public static bool IsHoldingCookableSingleItem()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
            return false;

        if (inventory.IsHoldingSingleItem && inventory.CurrentHeldItem != null && inventory.CurrentHeldItem.RequiresCooking)
            return true;

        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (slot.IsIngredient && slot.amount == PlayerInventory.SingleItemAmount && slot.ingredient.RequiresCooking)
                return true;
        }

        return false;
    }

    private void TryTakeIngredientFromStove(PlayerInventory inventory)
    {
        if (!inventory.SelectedSlot.IsEmpty)
            return;

        IngredientData itemToTake = isCooking ? rawIngredientOnStove : currentIngredientOnStove;
        if (itemToTake == null)
            itemToTake = currentIngredientOnStove;

        if (!inventory.TryAddIngredient(itemToTake, PlayerInventory.SingleItemAmount))
            return;

        GameAudioManager.Instance?.PlayTake();
        ResetStove();
    }

    private void TryPlaceHeldIngredientOnStove(PlayerInventory inventory)
    {
        if (!TryResolveCookableSingleItem(inventory, out IngredientData heldItem))
        {
            if (inventory.CurrentHeldItem != null && !inventory.CurrentHeldItem.RequiresCooking)
                AssemblyTable.Instance?.Interact();

            return;
        }

        if (!TryPlaceIngredient(heldItem))
            return;

        inventory.TryUseOneItem();
    }

    public bool TryPlaceIngredient(IngredientData ingredient)
    {
        if (currentIngredientOnStove != null || isCooking || ingredient == null)
            return false;

        if (!ingredient.RequiresCooking)
            return false;

        rawIngredientOnStove = ingredient;
        currentIngredientOnStove = ingredient;
        isBurned = false;
        GameAudioManager.Instance?.PlayPlace();
        StartCookingProcess();
        return true;
    }

    private void StartCookingProcess()
    {
        if (cookingCoroutine != null)
            StopCoroutine(cookingCoroutine);

        isCooking = true;
        stationAnimator?.SetStage(KitchenPrepStage.Frying);
        GameAudioManager.Instance?.StartStoveSizzle();
        OnCookingStarted?.Invoke();
        OnProgressUpdated?.Invoke(0f);
        cookingCoroutine = StartCoroutine(CookingCoroutine(rawIngredientOnStove));
    }

    private IEnumerator CookingCoroutine(IngredientData rawIngredient)
    {
        float elapsed = 0f;
        float targetTime = Mathf.Max(0.01f, rawIngredient.CookingTime);

        while (elapsed < targetTime)
        {
            if (!isCooking || currentIngredientOnStove == null)
                yield break;

            if (GameStateMachine.Instance != null &&
                GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
            {
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            OnProgressUpdated?.Invoke(Mathf.Clamp01(elapsed / targetTime));
            yield return null;
        }

        if (rawIngredient.CookedVersion != null && isCooking)
        {
            currentIngredientOnStove = rawIngredient.CookedVersion;
            stationAnimator?.SetStage(KitchenPrepStage.Done);
            GameAudioManager.Instance?.StopStoveSizzle();
            GameAudioManager.Instance?.PlayStoveDone();
            OnCookingFinished?.Invoke();
        }

        isCooking = false;
        cookingCoroutine = null;
        OnProgressUpdated?.Invoke(1f);
    }

    private void HandleOvercooking()
    {
        overcookTimer += Time.deltaTime;

        if (overcookTimer >= burnTime)
            BurnDish();
    }

    private void BurnDish()
    {
        if (currentIngredientOnStove == null)
            return;

        isBurned = true;
        isCooking = false;
        cookingCoroutine = null;
        GameAudioManager.Instance?.StopStoveSizzle();

        SessionStatistics.Instance?.AddSpoiledDish();

        stationAnimator?.SetStage(KitchenPrepStage.Burned);
        OnDishBurned?.Invoke();
        OnProgressUpdated?.Invoke(0f);
    }

    private void ResetStove()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        currentIngredientOnStove = null;
        rawIngredientOnStove = null;
        isCooking = false;
        isBurned = false;
        overcookTimer = 0f;
        GameAudioManager.Instance?.StopStoveSizzle();
        stationAnimator?.ResetToIdle();
        OnProgressUpdated?.Invoke(0f);
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        isUnderThreat = newState != GameStateMachine.GameState.Gameplay;
        stationAnimator?.SetUnderThreat(isUnderThreat);

        if (!isUnderThreat)
            overcookTimer = 0f;
    }

    private static bool TryResolveCookableSingleItem(PlayerInventory inventory, out IngredientData ingredient)
    {
        ingredient = null;

        if (inventory.IsHoldingSingleItem &&
            inventory.CurrentHeldItem != null &&
            inventory.CurrentHeldItem.RequiresCooking)
        {
            ingredient = inventory.CurrentHeldItem;
            return true;
        }

        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (!slot.IsIngredient || slot.amount != PlayerInventory.SingleItemAmount)
                continue;

            if (!slot.ingredient.RequiresCooking)
                continue;

            inventory.SelectSlot(i);
            ingredient = slot.ingredient;
            return true;
        }

        return false;
    }
}
