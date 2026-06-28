using System;
using System.Collections.Generic;
using UnityEngine;
using FrostbiteKitchen.Data;

[Serializable]
public struct InventorySlot
{
    public IngredientData ingredient;
    public DishData dish;
    public int amount;

    public bool IsEmpty => ingredient == null && dish == null;
    public bool IsIngredient => ingredient != null;
    public bool IsDish => dish != null;

    public static InventorySlot Empty => new InventorySlot();

    public static InventorySlot FromIngredient(IngredientData item, int count)
    {
        return new InventorySlot
        {
            ingredient = item,
            dish = null,
            amount = Mathf.Max(0, count)
        };
    }

    public static InventorySlot FromDish(DishData item)
    {
        return new InventorySlot
        {
            ingredient = null,
            dish = item,
            amount = 1
        };
    }

    public Sprite GetIcon()
    {
        if (IsDish)
            return dish != null ? dish.icon : null;

        if (!IsIngredient || ingredient == null)
            return null;

        if (amount > 1 && ingredient.packIcon != null)
            return ingredient.packIcon;

        return ingredient.icon;
    }

    public string GetDisplayName()
    {
        if (IsDish)
            return dish != null ? dish.dishName : "Nothing";

        if (IsIngredient)
            return ingredient != null ? ingredient.displayName : "Nothing";

        return "Nothing";
    }
}

public class PlayerInventory : MonoBehaviour
{
    public const int SlotCount = 4;
    public const int WarehousePackSize = 3;
    public const int SingleItemAmount = 1;

    public static PlayerInventory Instance { get; private set; }

    [SerializeField] private InventorySlot[] slots = new InventorySlot[SlotCount];
    [SerializeField] private int selectedSlotIndex;

    public delegate void OnHandChanged(IngredientData item, int amount);
    public event OnHandChanged OnHandUpdated;
    public event Action OnInventoryChanged;
    public event Action<int> OnSelectedSlotChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (slots == null || slots.Length != SlotCount)
            slots = new InventorySlot[SlotCount];
    }

    public int SelectedSlotIndex => selectedSlotIndex;
    public IReadOnlyList<InventorySlot> Slots => slots;

    public InventorySlot SelectedSlot => slots[selectedSlotIndex];

    public IngredientData CurrentHeldItem => SelectedSlot.ingredient;
    public int CurrentAmount => SelectedSlot.amount;
    public DishData CurrentHeldDish => SelectedSlot.dish;

    public bool IsEmpty => SelectedSlot.IsEmpty;
    public bool IsHoldingWarehousePack => SelectedSlot.IsIngredient && SelectedSlot.amount == WarehousePackSize;
    public bool IsHoldingSingleItem => SelectedSlot.IsIngredient && SelectedSlot.amount == SingleItemAmount;

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= SlotCount)
            return InventorySlot.Empty;

        return slots[index];
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= SlotCount || index == selectedSlotIndex)
            return;

        selectedSlotIndex = index;
        OnSelectedSlotChanged?.Invoke(selectedSlotIndex);
        OnHandUpdated?.Invoke(CurrentHeldItem, CurrentAmount);
        OnInventoryChanged?.Invoke();
    }

    public bool TryAddIngredient(IngredientData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        if (slots[selectedSlotIndex].IsEmpty)
        {
            SetSlot(selectedSlotIndex, InventorySlot.FromIngredient(item, amount));
            return true;
        }

        int emptySlot = FindEmptySlotIndex();
        if (emptySlot >= 0)
        {
            SetSlot(emptySlot, InventorySlot.FromIngredient(item, amount));
            SelectSlot(emptySlot);
            return true;
        }

        return false;
    }

    public bool TryAddDish(DishData item)
    {
        if (item == null)
            return false;

        if (slots[selectedSlotIndex].IsEmpty)
        {
            SetSlot(selectedSlotIndex, InventorySlot.FromDish(item));
            return true;
        }

        int emptySlot = FindEmptySlotIndex();
        if (emptySlot >= 0)
        {
            SetSlot(emptySlot, InventorySlot.FromDish(item));
            SelectSlot(emptySlot);
            return true;
        }

        return false;
    }

    public void SetHeldItem(IngredientData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            ClearSelectedSlot();
            return;
        }

        SetSlot(selectedSlotIndex, InventorySlot.FromIngredient(item, amount));
    }

    public bool TryUseOneItem()
    {
        return RemoveFromSlot(selectedSlotIndex, SingleItemAmount);
    }

    public bool RemoveItem(int amount)
    {
        return RemoveFromSlot(selectedSlotIndex, amount);
    }

    public void ClearInventory()
    {
        ClearSelectedSlot();
    }

    public void ClearSelectedSlot()
    {
        ClearSlot(selectedSlotIndex);
    }

    public void ClearSlot(int index)
    {
        if (index < 0 || index >= SlotCount)
            return;

        SetSlot(index, InventorySlot.Empty);
    }

    private bool RemoveFromSlot(int index, int amount)
    {
        if (index < 0 || index >= SlotCount || amount <= 0)
            return false;

        InventorySlot slot = slots[index];
        if (!slot.IsIngredient || slot.amount < amount)
            return false;

        int remaining = slot.amount - amount;
        if (remaining <= 0)
        {
            SetSlot(index, InventorySlot.Empty);
        }
        else
        {
            SetSlot(index, InventorySlot.FromIngredient(slot.ingredient, remaining));
        }

        return true;
    }

    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (slots[i].IsEmpty)
                return i;
        }

        return -1;
    }

    private void SetSlot(int index, InventorySlot slot)
    {
        if (index < 0 || index >= SlotCount)
            return;

        slots[index] = slot;

        if (index == selectedSlotIndex)
            OnHandUpdated?.Invoke(CurrentHeldItem, CurrentAmount);

        OnInventoryChanged?.Invoke();
    }
}
