using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

public class KitchenTableSlot : MonoBehaviour, IInteractable
{
    [Header("Настройки слота")]
    [SerializeField] private IngredientData allowedIngredient;
    [SerializeField] private int maxCount = 3;           // Теперь используется

    [Header("Текущее состояние")]
    [SerializeField] private int currentCount = 0;

    public delegate void OnStockChanged(int count);
    public event OnStockChanged OnStockUpdated;

    public void Interact()
    {
        var inventory = PlayerInventory.Instance;

        // === Разгрузка пачки (3 шт.) ===
        if (inventory.CurrentHeldItem != null && inventory.CurrentAmount == 3)
        {
            if (currentCount > 0)
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Слот уже занят!");
                return;
            }

            if (allowedIngredient == null || inventory.CurrentHeldItem == allowedIngredient)
            {
                currentCount = maxCount;                    // ← Используем maxCount
                allowedIngredient = inventory.CurrentHeldItem;

                inventory.ClearInventory();

                Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Разгружено {maxCount} шт. {allowedIngredient.displayName}");
                OnStockUpdated?.Invoke(currentCount);
            }
            else
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Неверный тип ингредиента для этого слота!");
            }
            return;
        }

        // === Взять 1 шт. ===
        if (inventory.CurrentHeldItem == null && currentCount > 0)
        {
            currentCount--;
            inventory.SetHeldItem(allowedIngredient, 1);

            Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Взят 1 шт. {allowedIngredient.displayName}. Осталось: {currentCount}");

            if (currentCount <= 0)
                allowedIngredient = null;

            OnStockUpdated?.Invoke(currentCount);
            return;
        }

        // Отправить в сборщик, если в руках 1 шт.
        if (inventory.CurrentHeldItem != null && inventory.CurrentAmount == 1)
        {
            if (DishAssembler.Instance != null)
            {
                DishAssembler.Instance.AddIngredient(inventory.CurrentHeldItem);
                inventory.ClearInventory();
            }
        }
    }

    public int CurrentCount => currentCount;
    public int MaxCount => maxCount;
}