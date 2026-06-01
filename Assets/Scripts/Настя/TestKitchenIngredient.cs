using UnityEngine;
using UnityEngine.EventSystems;

public class TestKitchenIngredient : MonoBehaviour, IPointerClickHandler
{
    [Header("Название ингредиента")]
    [SerializeField] private string ingredientName = "Ингредиент";

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"<color=orange>[КУХНЯ]</color> Взят ингредиент: <b>{ingredientName}</b>");
    }
}