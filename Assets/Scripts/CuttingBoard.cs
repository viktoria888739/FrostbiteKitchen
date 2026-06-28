using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenAnimation;

public class CuttingBoard : MonoBehaviour, IInteractable, IPointerClickHandler
{
    public static CuttingBoard Instance { get; private set; }

    [SerializeField] private KitchenStationAnimator stationAnimator;
    [SerializeField] private float burnTime = 11f;

    private IngredientData currentIngredient;
    private IngredientData rawIngredient;
    private bool isCutting;
    private bool isBurned;
    private float spoilTimer;
    private bool isUnderThreat;
    private Coroutine cuttingRoutine;
    private int lastInteractFrame = -1;

    public bool IsCutting => isCutting;
    public bool HasIngredient => currentIngredient != null;

    public event System.Action<float> OnProgressUpdated;
    public event System.Action OnCuttingStarted;
    public event System.Action OnCuttingFinished;
    public event System.Action OnIngredientBurned;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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
        if (!isCutting && currentIngredient != null && isUnderThreat && !isBurned)
        {
            spoilTimer += Time.deltaTime;
            if (spoilTimer >= burnTime)
                BurnIngredient();
        }
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
            ResetBoard();
            return;
        }

        if (currentIngredient != null)
        {
            TryTakeIngredient(inventory);
            return;
        }

        TryPlaceHeldIngredient(inventory);
    }

    public static bool CanAcceptHeldIngredient()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null || inventory.CurrentHeldDish != null)
            return false;

        return TryResolveCuttableSingleItem(inventory, out _);
    }

    public static bool IsHoldingCuttableSingleItem()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        return inventory != null && TryResolveCuttableSingleItem(inventory, out _);
    }

    public bool TryPlaceIngredient(IngredientData ingredient)
    {
        if (currentIngredient != null || isCutting || ingredient == null || !ingredient.RequiresCutting)
            return false;

        rawIngredient = ingredient;
        currentIngredient = ingredient;
        isBurned = false;
        spoilTimer = 0f;
        GameAudioManager.Instance?.PlayPlace();
        StartCuttingProcess();
        return true;
    }

    private void TryPlaceHeldIngredient(PlayerInventory inventory)
    {
        if (!TryResolveCuttableSingleItem(inventory, out IngredientData heldItem))
            return;

        if (!TryPlaceIngredient(heldItem))
            return;

        inventory.TryUseOneItem();
    }

    private void TryTakeIngredient(PlayerInventory inventory)
    {
        if (!inventory.SelectedSlot.IsEmpty)
            return;

        IngredientData itemToTake = isCutting ? rawIngredient : currentIngredient;
        if (itemToTake == null)
            itemToTake = currentIngredient;

        if (!inventory.TryAddIngredient(itemToTake, PlayerInventory.SingleItemAmount))
            return;

        GameAudioManager.Instance?.PlayTake();
        ResetBoard();
    }

    private void StartCuttingProcess()
    {
        if (cuttingRoutine != null)
            StopCoroutine(cuttingRoutine);

        isCutting = true;
        stationAnimator?.SetStage(KitchenPrepStage.Cutting);
        OnCuttingStarted?.Invoke();
        OnProgressUpdated?.Invoke(0f);
        cuttingRoutine = StartCoroutine(CuttingCoroutine(rawIngredient));
    }

    private IEnumerator CuttingCoroutine(IngredientData rawIngredientData)
    {
        float elapsed = 0f;
        float targetTime = Mathf.Max(0.01f, rawIngredientData.CuttingTime);

        while (elapsed < targetTime)
        {
            if (!isCutting || currentIngredient == null)
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

        if (rawIngredientData.CutVersion != null && isCutting)
        {
            currentIngredient = rawIngredientData.CutVersion;
            stationAnimator?.SetStage(KitchenPrepStage.Done);
            GameAudioManager.Instance?.PlayStoveDone();
            OnCuttingFinished?.Invoke();
        }

        isCutting = false;
        cuttingRoutine = null;
        OnProgressUpdated?.Invoke(1f);
    }

    private void BurnIngredient()
    {
        if (currentIngredient == null)
            return;

        isBurned = true;
        isCutting = false;
        cuttingRoutine = null;
        stationAnimator?.SetStage(KitchenPrepStage.Burned);
        OnIngredientBurned?.Invoke();
        OnProgressUpdated?.Invoke(0f);
    }

    private void ResetBoard()
    {
        if (cuttingRoutine != null)
        {
            StopCoroutine(cuttingRoutine);
            cuttingRoutine = null;
        }

        currentIngredient = null;
        rawIngredient = null;
        isCutting = false;
        isBurned = false;
        spoilTimer = 0f;
        stationAnimator?.ResetToIdle();
        OnProgressUpdated?.Invoke(0f);
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        isUnderThreat = newState != GameStateMachine.GameState.Gameplay;
        stationAnimator?.SetUnderThreat(isUnderThreat);

        if (!isUnderThreat)
            spoilTimer = 0f;
    }

    private static bool TryResolveCuttableSingleItem(PlayerInventory inventory, out IngredientData ingredient)
    {
        ingredient = null;

        if (inventory.IsHoldingSingleItem &&
            inventory.CurrentHeldItem != null &&
            inventory.CurrentHeldItem.RequiresCutting)
        {
            ingredient = inventory.CurrentHeldItem;
            return true;
        }

        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (!slot.IsIngredient || slot.amount != PlayerInventory.SingleItemAmount)
                continue;

            if (!slot.ingredient.RequiresCutting)
                continue;

            inventory.SelectSlot(i);
            ingredient = slot.ingredient;
            return true;
        }

        return false;
    }
}
