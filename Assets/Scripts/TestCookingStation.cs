using UnityEngine;
using UnityEngine.EventSystems;

public class TestCookingStation : MonoBehaviour, IPointerClickHandler
{
    [Header("Сообщение для консоли")]
    [SerializeField] private string logMessage = "Взаимодействие";

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"<color=red>[РАБОЧАЯ ЗОНА]</color> {logMessage}");
    }
}