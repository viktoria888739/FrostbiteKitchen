using UnityEngine;
using UnityEngine.EventSystems; // Подключаем систему отслеживания кликов по UI

public class DeliverySlotLog : MonoBehaviour, IPointerClickHandler
{
    [Header("Номер этого слота выдачи")]
    [Range(1, 3)]
    [SerializeField] private int slotIndex = 1;

    // Метод автоматически срабатывает в Unity при клике мышкой по серому квадрату
    public void OnPointerClick(PointerEventData eventData)
    {
        // Выводим в консоль именно ту строку, которую ты просила
        Debug.Log($"блюдо на стойке выдачи {slotIndex}");
    }
}