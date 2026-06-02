using UnityEngine;
using FrostbiteKitchen.Data;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

public class KitchenTableSlot : MonoBehaviour, IInteractable
{
    [Header("Что сейчас лежит на этой точке стола")]
    [SerializeField] private IngredientData storedIngredient;
    [SerializeField] private int currentStock = 0;

    public delegate void OnStockChanged(int count);
    public event OnStockChanged OnStockUpdated;

    public void Interact()
    {
        if (PlayerInventory.Instance.CurrentHeldItem != null && PlayerInventory.Instance.CurrentAmount > 1)
        {
            IngredientData itemsInHand = PlayerInventory.Instance.CurrentHeldItem;

            if (storedIngredient == null || storedIngredient == itemsInHand)
            {
                storedIngredient = itemsInHand;
                currentStock += PlayerInventory.Instance.CurrentAmount;
                
                Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Разгрузили со склада {PlayerInventory.Instance.CurrentAmount} шт. {storedIngredient.displayName}. Всего на столе: {currentStock}");
                
                PlayerInventory.Instance.ClearInventory(); 
                OnStockUpdated?.Invoke(currentStock);
            }
            else
            {
                Debug.LogWarning("[РАБОЧИЙ СТОЛ] Сюда нельзя положить этот ингредиент, место занято другим!");
            }
            return;
        }

        if (PlayerInventory.Instance.CurrentHeldItem == null && currentStock > 0)
        {
            currentStock--;
            PlayerInventory.Instance.SetHeldItem(storedIngredient, 1);
            
            Debug.Log($"<color=green>[РАБОЧИЙ СТОЛ]</color> Взяли 1 шт {storedIngredient.displayName} для готовки. Осталось на столе: {currentStock}");
            
            if (currentStock <= 0)
            {
                storedIngredient = null;
            }
            
            OnStockUpdated?.Invoke(currentStock);
            return;
        }

        if (PlayerInventory.Instance.CurrentHeldItem != null && PlayerInventory.Instance.CurrentAmount == 1)
        {
            if (DishAssembler.Instance != null)
            {
                Debug.Log($"<color=green>[ГОТОВКА]</color> Ингредиент {PlayerInventory.Instance.CurrentHeldItem.displayName} отправлен в сборщик блюда!");
                
                DishAssembler.Instance.AddIngredient(PlayerInventory.Instance.CurrentHeldItem);
                PlayerInventory.Instance.ClearInventory();
            }
        }
    }
}