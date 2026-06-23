using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private RecipeCatalog recipeCatalog;
    [SerializeField] private float timeBetweenOrders = 5f;

    [Header("Current Order")]
    [SerializeField] private RecipeData activeRecipe;
    private float currentOrderTimer;
    private bool isOrderActive = false;

    public static System.Action<RecipeData> OnNewOrderStarted;
    public static System.Action OnOrderExpired;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameStateMachine.Instance == null || 
            GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay) 
            return;

        if (isOrderActive)
        {
            UpdateOrderTimer();
        }
        else if (activeRecipe == null)
        {
            StartNewRandomOrder();
        }
    }

    public RecipeData GetActiveRecipe() => activeRecipe;

    public void StartNewRandomOrder()
    {
        if (recipeCatalog == null || recipeCatalog.AllRecipes.Count == 0) return;

        activeRecipe = recipeCatalog.AllRecipes[Random.Range(0, recipeCatalog.AllRecipes.Count)];
        currentOrderTimer = activeRecipe.timeLimit;
        isOrderActive = true;

        OnNewOrderStarted?.Invoke(activeRecipe);
        Debug.Log($"[OrderManager] Новый заказ: {activeRecipe.recipeName}");
    }

    private void UpdateOrderTimer()
    {
        currentOrderTimer -= Time.deltaTime;
        if (currentOrderTimer <= 0)
        {
            OrderFailed();
        }
    }

    private void OrderFailed()
    {
        isOrderActive = false;
        activeRecipe = null;

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddSpoiledDish();     // ← Исправлено

        Debug.Log("[OrderManager] Заказ провален по времени!");
        OnOrderExpired?.Invoke();

        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }

    public void CompleteActiveOrder()
    {
        if (!isOrderActive) return;

        Debug.Log($"[OrderManager] Заказ {activeRecipe.recipeName} выполнен!");

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddSuccessfulDish();   // ← Исправлено

        isOrderActive = false;
        activeRecipe = null;
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }
}