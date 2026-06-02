using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.Warehouse
{
    public class WarehouseBox : MonoBehaviour, IPointerClickHandler, IInteractable
    {
        [Header("Настройки коробки")]
        [SerializeField] private bool isEmpty = false;
        
        [SerializeField] private IngredientData ingredientData; 

        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (isEmpty || ingredientData == null)
            {
                Debug.Log("<color=gray>[СКЛАД]</color> Коробка пустая.");
            }
            else
            {
                Debug.Log($"<color=orange>[СКЛАД]</color> Взят ингредиент: <b>{ingredientData.displayName}</b> (ID: {ingredientData.ingredientId})");
            }
        }
    }
}