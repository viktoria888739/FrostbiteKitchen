using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.UI
{
    public class OrderCompleteBtn : MonoBehaviour, IInteractable
    {
        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
<<<<<<< Updated upstream
            if (dishAssembler == null || OrderManager.Instance == null)
            {
                Debug.LogError("[ЗОНА ВЫДАЧИ] Не найдены необходимые компоненты!");
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
                Debug.Log("<color=red>[ЗОНА ВЫДАЧИ]</color> Блюдо не соответствует заказу! Заказ провален.");

                OrderManager.Instance.FailCurrentOrder();
                dishAssembler.ClearPlate();
            }
=======
            OrderDelivery.TrySubmit();
>>>>>>> Stashed changes
        }
    }
}
