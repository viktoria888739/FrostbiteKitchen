using UnityEngine;
using UnityEngine.EventSystems;

public class TestOrderCompleteBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#33FFF3>[КНОПКА]</color> Заказ собран");
    }
}