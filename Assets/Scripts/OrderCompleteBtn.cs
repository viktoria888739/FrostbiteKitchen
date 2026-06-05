using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.UI
{
    public class OrderCompleteBtn : MonoBehaviour, IInteractable
    {
        [Header("Зона выдачи")]
        [Tooltip("Ссылка на сборщик (если не найдёт автоматически)")]
        [SerializeField] private DishAssembler dishAssembler;

        private void Awake()
        {
            if (dishAssembler == null)
            {
                dishAssembler = DishAssembler.Instance;
            }
        }

        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (dishAssembler == null)
            {
                Debug.LogError("[ЗОНА ВЫДАЧИ] DishAssembler не найден!");
                return;
            }

            if (OrderManager.Instance == null)
            {
                Debug.LogError("[ЗОНА ВЫДАЧИ] OrderManager не найден!");
                return;
            }

            RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
            if (activeRecipe == null)
            {
                Debug.LogWarning("<color=yellow>[ЗОНА ВЫДАЧИ]</color> Нет активного заказа.");
                return;
            }

            if (dishAssembler.ValidateRecipe(activeRecipe))
            {
                Debug.Log($"<color=green>[ЗОНА ВЫДАЧИ]</color> Заказ '{activeRecipe.recipeName}' успешно сдан!");

                OrderManager.Instance.CompleteActiveOrder();
                dishAssembler.ClearPlate();
            }
            else
            {
                Debug.Log("<color=red>[ЗОНА ВЫДАЧИ]</color> Блюдо не соответствует заказу! Проверьте ингредиенты.");
            }
        }
    }
}