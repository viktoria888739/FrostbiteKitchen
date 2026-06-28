using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

public class KitchenTableSlot : MonoBehaviour, IInteractable
{
    [Header("Настройки слота")]
    [SerializeField] private IngredientData allowedIngredient;
    [SerializeField] private int maxCount = 3;

    [Header("Текущее состояние")]
    [SerializeField] private int currentCount = 0;

    [Header("UI Визуал ячейки стола")]
    [Tooltip("Перетащи сюда дочерний объект с твоей картинкой ячейки, которая должна появляться/исчезать")]
    [SerializeField] private GameObject slotVisualObject; // Заменили Image на GameObject, чтобы управлять только видимостью

    public delegate void OnStockChanged(int count);
    public event OnStockChanged OnStockUpdated;

    private void Start()
    {
        // На старте игры проверяем, нужно ли показать или скрыть ячейку
        UpdateVisual();
    }

    public void Interact()
    {
        var inventory = PlayerInventory.Instance;

        // Если в руках у игрока пачка (3 шт.) — разгружаем её на стол
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
                UpdateVisual(); // Показываем твою картинку ячейки
            }
            else
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Неверный тип ингредиента для этого слота!");
            }
            return;
        }

        // Если руки пустые, а на столе что-то есть — берем 1 штуку со стола в руки
        if (inventory.CurrentHeldItem == null && currentCount > 0)
        {
            currentCount--;
            inventory.SetHeldItem(allowedIngredient, 1);

            Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Взят 1 шт. {allowedIngredient.displayName}. Осталось: {currentCount}");

            if (currentCount <= 0)
                allowedIngredient = null;

            OnStockUpdated?.Invoke(currentCount);
            UpdateVisual(); // Скрываем картинку ячейки, если ингредиенты закончились
            return;
        }

        // Если в руках уже есть 1 штука — отдаем её на сборочный стол
        if (inventory.CurrentHeldItem != null && inventory.CurrentAmount == 1)
        {
            if (DishAssembler.Instance != null)
            {
                DishAssembler.Instance.AddIngredient(inventory.CurrentHeldItem);
                inventory.ClearInventory();
            }
        }
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }

    // Логика видимости: просто включаем или выключаем объект, не меняя в нем картинку
    private void UpdateVisual()
    {
        if (slotVisualObject == null) return;

        // Если в ячейке есть предметы (количество больше 0), включаем твою картинку
        if (currentCount > 0)
        {
            slotVisualObject.SetActive(true); // Объект становится видимым (показывает твою картинку)
        }
        else
        {
            slotVisualObject.SetActive(false); // Объект полностью отключается (картинка пропадает)
        }
    }
}