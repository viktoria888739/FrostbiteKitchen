using UnityEngine;
using UnityEngine.EventSystems;

public class DeliverySlotLog : MonoBehaviour, IPointerClickHandler
{
    [Header("����� ����� ����� ������")]
    [Range(1, 3)]
    [SerializeField] private int slotIndex = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"����� �� ������ ������ {slotIndex}");
    }
}