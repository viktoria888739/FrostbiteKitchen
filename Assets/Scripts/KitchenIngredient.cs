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

        Debug.Log($"<color=orange>[КУХНЯ]</color> Взят ингредиент: <b>{ingredientData.displayName}</b>");
        
        if (DishAssembler.Instance != null)
        {
            DishAssembler.Instance.AddIngredient(ingredientData);
        }
    }

    private void OnMouseDown()
    {
        Interact();
    }
}