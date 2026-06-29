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

        if (!inventory.TryAddIngredient(ingredientData, PlayerInventory.SingleItemAmount))
        {
            Debug.Log("<color=yellow>[КУХНЯ]</color> Инвентарь полон! Освободи слот.");
            return;
        }

        GameAudioManager.Instance?.PlayTake();

        Debug.Log($"<color=orange>[КУХНЯ]</color> Взят ингредиент: <b>{ingredientData.displayName}</b>");
    }
}