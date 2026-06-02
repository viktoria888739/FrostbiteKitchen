using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.KitchenStation
{
    public class Stove : MonoBehaviour, IPointerClickHandler, IInteractable
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (!gameObject.activeInHierarchy) return;

            Debug.Log("<color=#FF5733>[ПЛИТА]</color> Взаимодействие с плитой. Начинаем готовку/нагрев.");
        }
    }
}