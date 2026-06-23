using UnityEngine;
using FrostbiteKitchen.Data;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("Что сейчас в руках")]
    [SerializeField] private IngredientData currentHeldItem;
    [SerializeField] private int currentAmount = 0;

    public delegate void OnHandChanged(IngredientData item, int amount);
    public event OnHandChanged OnHandUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IngredientData CurrentHeldItem => currentHeldItem;
    public int CurrentAmount => currentAmount;

    public void SetHeldItem(IngredientData item, int amount)
    {
        currentHeldItem = item;
        currentAmount = Mathf.Max(0, amount);
        
        OnHandUpdated?.Invoke(currentHeldItem, currentAmount);
        
        Debug.Log($"<color=cyan>[ИНВЕНТАРЬ]</color> В руках: { (item != null ? item.displayName : "Nothing") } ({currentAmount} шт.)");
    }

    public bool TryUseOneItem()
    {
        if (currentHeldItem == null || currentAmount <= 0) return false;

        currentAmount--;
        
        if (currentAmount <= 0)
        {
            ClearInventory();
        }
        else
        {
            OnHandUpdated?.Invoke(currentHeldItem, currentAmount);
        }
        return true;
    }

    /// <summary>
    /// Новый метод по ТЗ — уменьшить количество на N штук
    /// </summary>
    public void RemoveItem(int amount)
    {
        if (currentHeldItem == null) return;
        
        currentAmount -= Mathf.Max(0, amount);
        
        if (currentAmount <= 0)
        {
            ClearInventory();
        }
        else
        {
            OnHandUpdated?.Invoke(currentHeldItem, currentAmount);
        }
    }

    public void ClearInventory()
    {
        currentHeldItem = null;
        currentAmount = 0;
        OnHandUpdated?.Invoke(null, 0);
        Debug.Log("<color=cyan>[ИНВЕНТАРЬ]</color> Руки теперь пусты");
    }
}