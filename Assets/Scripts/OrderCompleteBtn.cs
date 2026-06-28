using UnityEngine;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.UI{
    public class OrderCompleteBtn : MonoBehaviour, IInteractable
    {
        public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            OrderDelivery.TrySubmit();
        }
    }
}
