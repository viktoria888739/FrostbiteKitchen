using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.KitchenStation;

public class KitchenTableSlot : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData allowedIngredient;
    [SerializeField] private int maxCount = PlayerInventory.WarehousePackSize;
    [SerializeField] private int currentCount = 0;
<<<<<<< Updated upstream
=======
    [SerializeField] private GameObject slotVisualObject;
>>>>>>> Stashed changes

    public delegate void OnStockChanged(int count);
    public event OnStockChanged OnStockUpdated;

<<<<<<< Updated upstream
=======
    public IngredientData AllowedIngredient => allowedIngredient;
    public int CurrentCount => currentCount;

    private void Start()
    {
        UpdateVisual();
    }

>>>>>>> Stashed changes
    public void Interact()
    {
        if (PlayerInventory.Instance == null || allowedIngredient == null)
            return;

        var inventory = PlayerInventory.Instance;

<<<<<<< Updated upstream
        if (inventory.CurrentHeldItem != null && inventory.CurrentAmount == 3)
        {
            if (currentCount > 0)
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Слот уже занят!");
                return;
            }

            if (allowedIngredient == null || inventory.CurrentHeldItem == allowedIngredient)
            {
                currentCount = maxCount;
                allowedIngredient = inventory.CurrentHeldItem;

                inventory.ClearInventory();

                Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Разгружено {maxCount} шт. {allowedIngredient.displayName}");
                OnStockUpdated?.Invoke(currentCount);
            }
            else
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Неверный тип ингредиента для этого слота!");
            }
=======
        if (TryUnloadWarehousePack(inventory))
>>>>>>> Stashed changes
            return;

<<<<<<< Updated upstream
        if (inventory.CurrentHeldItem == null && currentCount > 0)
        {
            currentCount--;
            inventory.SetHeldItem(allowedIngredient, 1);

            Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Взят 1 шт. {allowedIngredient.displayName}. Осталось: {currentCount}");

            if (currentCount <= 0)
                allowedIngredient = null;

            OnStockUpdated?.Invoke(currentCount);
=======
        if (TryTakeSingleItemForCooking(inventory))
>>>>>>> Stashed changes
            return;

        if (TryPlaceHeldItemOnAssembly(inventory))
            return;
    }

    private bool TryUnloadWarehousePack(PlayerInventory inventory)
    {
        if (!inventory.IsHoldingWarehousePack)
        {
            return false;
        }

<<<<<<< Updated upstream
        if (inventory.CurrentHeldItem != null && inventory.CurrentAmount == 1)
=======
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
        if (currentCount <= 0)
            return false;

        currentCount--;

        if (!inventory.TryAddIngredient(allowedIngredient, PlayerInventory.SingleItemAmount))
>>>>>>> Stashed changes
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

        bool hasMatchingIngredient = false;
        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            InventorySlot slot = inventory.GetSlot(i);
            if (!slot.IsIngredient || slot.ingredient != allowedIngredient || slot.ingredient.RequiresCutting)
                continue;

            inventory.SelectSlot(i);
            hasMatchingIngredient = true;
            break;
        }

        if (!hasMatchingIngredient || AssemblyTable.Instance == null)
            return false;

        AssemblyTable.Instance.Interact();
        return true;
    }

<<<<<<< Updated upstream
    public int CurrentCount => currentCount;
    public int MaxCount => maxCount;
}
=======
    public int GetCurrentCount()
    {
        return currentCount;
    }

    private void UpdateVisual()
    {
        if (slotVisualObject == null)
        {
            return;
        }

        slotVisualObject.SetActive(currentCount > 0);
    }
}
>>>>>>> Stashed changes
