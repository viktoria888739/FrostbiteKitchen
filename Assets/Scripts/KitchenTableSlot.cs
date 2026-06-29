using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenStation;

public class KitchenTableSlot : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData allowedIngredient;
    [SerializeField] private int maxCount = PlayerInventory.WarehousePackSize;
    [SerializeField] private int currentCount = 0;
    [SerializeField] private GameObject slotVisualObject;

    public delegate void OnStockChanged(int count);
    public event OnStockChanged OnStockUpdated;

    public IngredientData AllowedIngredient => allowedIngredient;
    public int CurrentCount => currentCount;

    private void Start()
    {
        UpdateVisual();
    }

    public void Interact()
    {
        if (PlayerInventory.Instance == null || allowedIngredient == null)
            return;

        PlayerInventory inventory = PlayerInventory.Instance;

        if (TryUnloadWarehousePack(inventory))
            return;

        if (TryTakeSingleItemForCooking(inventory))
            return;

        if (TryPlaceHeldItemOnAssembly(inventory))
            return;
    }

    private bool TryUnloadWarehousePack(PlayerInventory inventory)
    {
        if (!inventory.IsHoldingWarehousePack)
            return false;

        if (inventory.CurrentHeldItem != allowedIngredient)
            return true;

        if (currentCount > 0)
            return true;

        currentCount = maxCount;
        inventory.ClearInventory();
        OnStockUpdated?.Invoke(currentCount);
        UpdateVisual();
        return true;
    }

    private bool TryTakeSingleItemForCooking(PlayerInventory inventory)
    {
        if (currentCount <= 0 || !inventory.HasEmptySelectedSlot)
            return false;

        currentCount--;

        if (!inventory.TryAddIngredient(allowedIngredient, PlayerInventory.SingleItemAmount))
        {
            currentCount++;
            return true;
        }

        GameAudioManager.Instance?.PlayTake();
        OnStockUpdated?.Invoke(currentCount);
        UpdateVisual();
        return true;
    }

    private bool TryPlaceHeldItemOnAssembly(PlayerInventory inventory)
    {
        if (inventory.CurrentHeldDish != null || inventory.IsHoldingWarehousePack || currentCount > 0)
            return false;

        if (!inventory.TryGetSelectedSingleIngredient(out IngredientData ingredient))
            return false;

        if (ingredient != allowedIngredient || AssemblyTable.Instance == null)
            return false;

        AssemblyTable.Instance.Interact();
        return true;
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    private void UpdateVisual()
    {
        if (slotVisualObject == null)
            return;

        slotVisualObject.SetActive(currentCount > 0);
    }
}
