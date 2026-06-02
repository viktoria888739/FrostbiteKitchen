using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.UI
{
    public class OrderCompleteBtn : MonoBehaviour, IPointerClickHandler, IInteractable
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (!gameObject.activeInHierarchy) return;

            Debug.Log("<color=#33FFF3>[КНОПКА]</color> Запрос на завершение и отдачу текущего заказа.");
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.CompleteActiveOrder();
            }
        }
    }
}