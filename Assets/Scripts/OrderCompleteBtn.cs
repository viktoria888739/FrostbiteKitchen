using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.UI
{
    public class OrderCompleteBtn : MonoBehaviour, IInteractable, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            OrderDelivery.TrySubmit();
        }
    }
}
