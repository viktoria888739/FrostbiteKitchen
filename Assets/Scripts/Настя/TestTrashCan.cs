using UnityEngine;
using UnityEngine.EventSystems;

public class TestTrashCan : MonoBehaviour, IPointerClickHandler
{
    // Метод автоматически сработает при клике мышкой по объекту мусорки в UI
    public void OnPointerClick(PointerEventData eventData)
    {
        // Проверяем, активен ли объект на сцене
        if (!gameObject.activeInHierarchy) return;

        // Выводим стильное сообщение в консоль
        Debug.Log("<color=#808080>[МУСОРКА]</color> Взаимодействие с мусоркой. Содержимое тарелки выброшено.");
    }
}