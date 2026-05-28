using UnityEngine;
using System;

public class HandItemManager : MonoBehaviour
{
    public static HandItemManager Instance { get; private set; }
    public event Action<IngredientData> OnHandItemChanged;

    [SerializeField] private IngredientData currentItem;
    public IngredientData CurrentItem => currentItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        Debug.Log("[HandItemManager] Awake");
    }

    public void SetHandItem(IngredientData newItem)
    {
        if (currentItem == newItem) return;
        currentItem = newItem;
        OnHandItemChanged?.Invoke(currentItem);
        Debug.Log($"[HandItemManager] Предмет в руках: {currentItem?.displayName ?? "ничего"}");
    }

    public void ClearHandItem() => SetHandItem(null);
}