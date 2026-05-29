using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class OrderCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dishNameText;
    [SerializeField] private TextMeshProUGUI timerText;  
    [SerializeField] private Image backgroundImage;

    private RecipeData currentRecipe;
    
    private void Awake()
    {
        Debug.Log("[OrderCard] Awake");
        if (dishNameText == null) Debug.LogError("[OrderCard] dishNameText не назначен!");
        if (timerText == null) Debug.LogError("[OrderCard] timerText не назначен!");
    }

    public void Setup(RecipeData recipe)
    {
        Debug.Log($"[OrderCard] Setup для рецепта: {recipe.displayName}");
        currentRecipe = recipe;
        if (dishNameText != null)
            dishNameText.text = recipe.displayName;
    }

    private void Update()
    {
        if (currentRecipe == null)
        {
            return;
        }

        if (OrderManager.Instance == null)
        {
            Debug.LogError("[OrderCard] OrderManager.Instance == null!");
            return;
        }

        if (OrderManager.Instance.IsOrderActive)
        {
            float remaining = OrderManager.Instance.GetCurrentOrderRemainingTime();
            UpdateTimerDisplay(remaining);
        }
    }

    private void UpdateTimerDisplay(float seconds)
    {
        if (timerText == null) return;
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        timerText.text = $"{time:mm\\:ss}";
        //менять цвет при приближении таймера к 0
        if (seconds <= 5f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }
}