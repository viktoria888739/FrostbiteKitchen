using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Ссылка на общую базу данных рецептов (RecipeCatalog)")]
    [SerializeField] private RecipeCatalog recipeCatalog;

    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private float timeBetweenOrders = 5f;

    [Header("Current Order Info")]
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
        if (GameStateMachine.Instance == null || GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay) return;

        if (isOrderActive)
        {
            UpdateOrderTimer();
        }
        else
        {
            if (activeRecipe == null && !IsInvoking(nameof(StartNewRandomOrder)))
            {
                StartNewRandomOrder();
            }
        }
    }
    public RecipeData GetActiveRecipe()
    {
        return activeRecipe;
    }
    public void StartNewRandomOrder()
    {
        if (recipeCatalog == null || recipeCatalog.AllRecipes == null || recipeCatalog.AllRecipes.Count == 0)
        {
            Debug.LogWarning("[OrderManager] RecipeCatalog не назначен в инспекторе или пуст!");
            return;
        }

        List<RecipeData> availableRecipes = recipeCatalog.AllRecipes;

        activeRecipe = availableRecipes[Random.Range(0, availableRecipes.Count)];
        currentOrderTimer = activeRecipe.timeLimit;
        isOrderActive = true;
        OnNewOrderStarted?.Invoke(activeRecipe);
        Debug.Log($"[OrderManager] Новый заказ запущен из каталога: {activeRecipe.recipeName}");
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
        OnOrderExpired?.Invoke();

        Debug.Log("[OrderManager] Время вышло! Заказ провален.");

        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }

    public void CompleteActiveOrder()
    {
        if (!isOrderActive) return;
        Debug.Log($"[OrderManager] Заказ {activeRecipe.recipeName} успешно выполнен!");
        isOrderActive = false;
        activeRecipe = null;
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }
}
