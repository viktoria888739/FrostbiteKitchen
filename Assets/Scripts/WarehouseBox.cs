using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

public class WarehouseBox : MonoBehaviour, IInteractable
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private int amountToGive = PlayerInventory.WarehousePackSize;
    [SerializeField] private bool isEmpty = false;

    public void Interact()
    {
        if (isEmpty || ingredientData == null || PlayerInventory.Instance == null)
            return;

        if (!PlayerInventory.Instance.TryAddIngredient(ingredientData, amountToGive))
            return;

        GameAudioManager.Instance?.PlayTake();
    }
}
