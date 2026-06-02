using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTable : MonoBehaviour, IPointerClickHandler, IInteractable
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (!gameObject.activeInHierarchy) return;

            Debug.Log("<color=#33FF57>[СТОЛ СБОРКИ]</color> Взаимодействие с рабочей зоной сборки блюд.");
        }
    }
}