using UnityEngine;
using UnityEngine.EventSystems;

public class TestBlindsToggle : MonoBehaviour, IPointerClickHandler
{
    private RectTransform rectTransform;
    private bool isClosed = false;

    // Параметры для двух состояний (настраиваются в инспекторе)
    [Header("Высота жалюзи")]
    [SerializeField] private float openHeight = 60f;    // Узкая полоска наверху
    [SerializeField] private float closedHeight = 450f; // Закрывает всё окно

    void Start()
    {
        // Получаем доступ к размерам UI-элемента
        rectTransform = GetComponent<RectTransform>();
        SetBlindsHeight(openHeight);
    }

    // Метод срабатывает при клике на жалюзи
    public void OnPointerClick(PointerEventData eventData)
    {
        isClosed = !isClosed; // Переключаем состояние (открыто/закрыто)

        if (isClosed)
        {
            SetBlindsHeight(closedHeight);
            Debug.Log("<color=red>[ЖАЛЮЗИ]</color> Окно выдачи ЗАКРЫТО");
        }
        else
        {
            SetBlindsHeight(openHeight);
            Debug.Log("<color=green>[ЖАЛЮЗИ]</color> Окно выдачи ОТКРЫТО");
        }
    }

    // Вспомогательный метод для изменения высоты прямоугольника
    private void SetBlindsHeight(float height)
    {
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
        }
    }
}