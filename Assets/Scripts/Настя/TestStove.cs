using UnityEngine;
using UnityEngine.EventSystems;

public class TestStove : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#FF5733>[ПЛИТА]</color> Взаимодействие с плитой");
    }
}