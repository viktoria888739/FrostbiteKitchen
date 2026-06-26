using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

public class KitchenIngredient : MonoBehaviour, IInteractable
{
    [Header("Данные ингредиента")]
    [SerializeField] private IngredientData ingredientData;

    public void Interact()
    {
        if (ingredientData == null)
        {
            Debug.LogWarning($"[КУХНЯ] На объекте {gameObject.name} не назначены IngredientData!");
            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("[КУХНЯ] Критическая ошибка: На сцене нет PlayerInventory!");
            return;
        }

        var inventory = PlayerInventory.Instance;

        if (inventory.CurrentHeldItem != null && inventory.CurrentHeldItem != ingredientData)
        {
            Debug.Log("<color=yellow>[КУХНЯ]</color> Руки заняты другим предметом! Сначала освободи их.");
            return;
        }

        inventory.SetHeldItem(ingredientData, 1);

        Debug.Log($"<color=orange>[КУХНЯ]</color> Взят в руки ингредиент: <b>{ingredientData.displayName}</b>");
    }
}