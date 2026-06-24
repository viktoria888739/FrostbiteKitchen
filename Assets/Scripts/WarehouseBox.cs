using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

public class WarehouseBox : MonoBehaviour, IInteractable
{
    [Header("Настройки склада")]
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private int amountToGive = 3;
    [SerializeField] private bool isEmpty = false;

    public void Interact()
    {
        if (isEmpty || ingredientData == null)
        {
            Debug.Log($"[СКЛАД] Коробка {gameObject.name} пуста.");
            return;
        }

        if (PlayerInventory.Instance.CurrentHeldItem != null)
        {
            Debug.Log("<color=yellow>[СКЛАД]</color> Руки заняты! Сначала разгрузи на стол.");
            return;
        }

        PlayerInventory.Instance.SetHeldItem(ingredientData, amountToGive);
        Debug.Log($"<color=orange>[СКЛАД]</color> Взята пачка: {ingredientData.displayName} x{amountToGive}");
    }
}