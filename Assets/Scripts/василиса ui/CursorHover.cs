using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Vector2 hotSpot = new Vector2(16, 16);

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
}