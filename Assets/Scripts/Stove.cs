using System.Collections;
using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenAnimation;
using FrostbiteKitchen.KitchenStation;

public class Stove : MonoBehaviour, IInteractable
{
    [SerializeField] private KitchenStationAnimator stationAnimator;
    [SerializeField] private StoveSurfaceVisual surfaceVisual;
    [SerializeField] private float pickupWindowSeconds = 3f;

    private IngredientData rawIngredientOnStove;
    private bool isCooking;
    private bool isBurned;
    private float cookingElapsed;
    private float cookingTotalTime;
    private Coroutine cookingCoroutine;
    private int lastInteractFrame = -1;

    public bool IsCooking => isCooking;
    public bool HasIngredientOnStove => rawIngredientOnStove != null || isBurned;

    public event System.Action<float> OnProgressUpdated;
    public event System.Action OnCookingStarted;
    public event System.Action OnIngredientBurned;
    public event System.Action OnStoveCleared;

    private void Awake()
    {
        if (stationAnimator == null)
            stationAnimator = GetComponent<KitchenStationAnimator>();

        if (surfaceVisual == null)
            surfaceVisual = StoveSurfaceVisual.EnsureOnStove(this);

        ImageRaycastHelper.EnsureRaycastTarget(gameObject);
    }

    private void Start()
    {
        surfaceVisual?.WireProgressBridges(this);
        ResetStove();
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

        if (rawIngredientOnStove != null)
        {
            if (isCooking)
                TryPickupCookedIngredient(inventory);

            return;
        }

        TryPlaceHeldIngredientOnStove(inventory);
    }

    public static bool IsHoldingCookableSingleItem()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null)
            return false;

        if (inventory.IsHoldingSingleItem &&
            inventory.CurrentHeldItem != null &&
            inventory.CurrentHeldItem.RequiresCooking)
        {
            return true;
        }

        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (slot.IsIngredient &&
                slot.amount == PlayerInventory.SingleItemAmount &&
                slot.ingredient.RequiresCooking)
            {
                return true;
            }
        }

        return false;
    }

    public bool CanPickupCookedIngredient()
    {
        return isCooking && IsWithinPickupWindow();
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
        if (rawIngredientOnStove != null || isCooking || ingredient == null)
            return false;

        if (!ingredient.RequiresCooking)
            return false;

        rawIngredientOnStove = ingredient;
        isBurned = false;
        cookingElapsed = 0f;
        cookingTotalTime = Mathf.Max(0.01f, ingredient.CookingTime);

        surfaceVisual?.ShowIngredient(ingredient.icon);
        surfaceVisual?.SetProgressBarVisible(true);
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
        cookingCoroutine = StartCoroutine(CookingCoroutine());
    }

    private IEnumerator CookingCoroutine()
    {
        while (cookingElapsed < cookingTotalTime)
        {
            if (!isCooking || rawIngredientOnStove == null)
                yield break;

            if (GameStateMachine.Instance == null ||
                GameStateMachine.Instance.CurrentState == GameStateMachine.GameState.Gameplay)
            {
                cookingElapsed += Time.deltaTime;
            }

            OnProgressUpdated?.Invoke(Mathf.Clamp01(cookingElapsed / cookingTotalTime));
            yield return null;
        }

        if (isCooking && rawIngredientOnStove != null)
            BurnFromTimeout();

        cookingCoroutine = null;
    }

    private void TryPickupCookedIngredient(PlayerInventory inventory)
    {
        if (!IsWithinPickupWindow())
            return;

        IngredientData cooked = rawIngredientOnStove != null ? rawIngredientOnStove.CookedVersion : null;
        if (cooked == null)
            return;

        if (!inventory.TryAddIngredient(cooked, PlayerInventory.SingleItemAmount))
            return;

        SelectSlotWithIngredient(inventory, cooked);

        GameAudioManager.Instance?.PlayTake();
        GameAudioManager.Instance?.PlayStoveDone();
        stationAnimator?.SetStage(KitchenPrepStage.Done);
        ResetStove();
    }

    private static void SelectSlotWithIngredient(PlayerInventory inventory, IngredientData ingredient)
    {
        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (!slot.IsIngredient || slot.ingredient != ingredient)
                continue;

            inventory.SelectSlot(i);
            return;
        }
    }

    private bool IsWithinPickupWindow()
    {
        if (!isCooking || rawIngredientOnStove == null)
            return false;

        float pickupStartTime = Mathf.Max(0f, cookingTotalTime - pickupWindowSeconds);
        return cookingElapsed >= pickupStartTime && cookingElapsed < cookingTotalTime;
    }

    private void BurnFromTimeout()
    {
        if (rawIngredientOnStove == null)
            return;

        isBurned = true;
        isCooking = false;

        GameAudioManager.Instance?.StopStoveSizzle();
        SessionStatistics.Instance?.AddSpoiledDish();
        stationAnimator?.SetStage(KitchenPrepStage.Burned);
        OnIngredientBurned?.Invoke();
        OnProgressUpdated?.Invoke(1f);

        surfaceVisual?.HideIngredient();
        StartCoroutine(ClearBurnedAfterDelay(0.6f));
    }

    private IEnumerator ClearBurnedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetStove();
    }

    private void ResetStove()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        rawIngredientOnStove = null;
        isCooking = false;
        isBurned = false;
        cookingElapsed = 0f;
        cookingTotalTime = 0f;

        GameAudioManager.Instance?.StopStoveSizzle();
        stationAnimator?.ResetToIdle();
        surfaceVisual?.HideIngredient();
        surfaceVisual?.SetProgressBarVisible(false);
        OnProgressUpdated?.Invoke(0f);
        OnStoveCleared?.Invoke();
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
