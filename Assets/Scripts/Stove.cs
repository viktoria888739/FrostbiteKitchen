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
    private float burnedClearAt = -1f;
    private int lastInteractFrame = -1;

    public bool IsCooking => isCooking;
    public bool HasIngredientOnStove => rawIngredientOnStove != null || isBurned;

    public float GetCookingProgressNormalized()
    {
        if (cookingTotalTime <= 0f)
            return 0f;

        return Mathf.Clamp01(cookingElapsed / cookingTotalTime);
    }

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

        return inventory.TryGetSelectedSingleIngredient(out _, ingredient => ingredient.RequiresCooking);
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
        isCooking = true;
        stationAnimator?.SetStage(KitchenPrepStage.Frying);
        GameAudioManager.Instance?.StartStoveSizzle();
        OnCookingStarted?.Invoke();
        OnProgressUpdated?.Invoke(GetCookingProgressNormalized());
        StoveCookingRunner.EnsureExists().Register(this);
    }

    public void TickCooking(float deltaTime)
    {
        if (burnedClearAt > 0f && Time.time >= burnedClearAt)
        {
            burnedClearAt = -1f;
            ResetStove();
            return;
        }

        if (!isCooking || rawIngredientOnStove == null)
            return;

        cookingElapsed += deltaTime;
        float progress = GetCookingProgressNormalized();
        OnProgressUpdated?.Invoke(progress);

        if (cookingElapsed >= cookingTotalTime)
            BurnFromTimeout();
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

        GameAudioManager.Instance?.PlayTake();
        GameAudioManager.Instance?.PlayStoveDone();
        stationAnimator?.SetStage(KitchenPrepStage.Done);
        ResetStove();
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
        burnedClearAt = Time.time + 0.6f;
    }

    private void ResetStove()
    {
        StoveCookingRunner.Instance?.Unregister(this);

        rawIngredientOnStove = null;
        isCooking = false;
        isBurned = false;
        cookingElapsed = 0f;
        cookingTotalTime = 0f;
        burnedClearAt = -1f;

        GameAudioManager.Instance?.StopStoveSizzle();
        stationAnimator?.ResetToIdle();
        surfaceVisual?.HideIngredient();
        surfaceVisual?.SetProgressBarVisible(false);
        OnProgressUpdated?.Invoke(0f);
        OnStoveCleared?.Invoke();
    }

    private static bool TryResolveCookableSingleItem(PlayerInventory inventory, out IngredientData ingredient)
    {
        return inventory.TryGetSelectedSingleIngredient(out ingredient, item => item.RequiresCooking);
    }
}
