using UnityEngine;
using UnityEngine.EventSystems;

public class TestWarehouseBox : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки коробки")]
    [SerializeField] private bool isEmpty = false;
    [SerializeField] private string ingredientName = "Картошка";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isEmpty)
        {
            Debug.Log("<color=gray>[СКЛАД]</color> Коробка пустая");
        }
        else
        {
            Debug.Log($"<color=orange>[СКЛАД]</color> Ингредиент взят: <b>{ingredientName}</b>");
        }
    }
}