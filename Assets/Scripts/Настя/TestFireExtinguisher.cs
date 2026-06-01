using UnityEngine;
using UnityEngine.EventSystems;

public class TestFireExtinguisher : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#FF3333>[ОГНЕТУШИТЕЛЬ]</color> Взаимодействие с огнетушителем");
    }
}