using UnityEngine;
using UnityEngine.EventSystems;

public class TestAssemblyTable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#33FF57>[СТОЛ СБОРКИ]</color> Взаимодействие с рабочей зоной");
    }
}