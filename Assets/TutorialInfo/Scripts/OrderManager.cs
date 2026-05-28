using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Settings")]
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
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        Debug.Log("[OrderManager] Awake, Instance установлен");
    }

    private void Start()
    {
        Debug.Log($"[OrderManager] Start, количество рецептов: {allRecipes?.Count ?? 0}");
        if (allRecipes == null || allRecipes.Count == 0)
            Debug.LogError("[OrderManager] Список allRecipes пуст! Добавьте рецепты в инспекторе.");
    }

    private void Update()
    {

        // Временно для теста можно закомментировать проверку состояния
        if (GameStateMachine.Instance != null && GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
        {
            Debug.Log("[OrderManager] Состояние не Gameplay, заказы не создаются");
            return;
        }

        if (isOrderActive)
        {
            UpdateOrderTimer();
        }
        else
        {
            if (activeRecipe == null)
            {
                Debug.Log("[OrderManager] Активного заказа нет, запускаю новый случайный заказ");
                StartNewRandomOrder();
            }
        }
    }

    public void StartNewRandomOrder()
    {
        if (allRecipes == null || allRecipes.Count == 0)
        {
            Debug.LogError("[OrderManager] Невозможно создать заказ: список рецептов пуст!");
            return;
        }

        activeRecipe = allRecipes[Random.Range(0, allRecipes.Count)];
        currentOrderTimer = activeRecipe.timeLimit;
        isOrderActive = true;

        OnNewOrderStarted?.Invoke(activeRecipe);
        Debug.Log($"[OrderManager] Новый заказ: {activeRecipe.displayName}, лимит времени: {activeRecipe.timeLimit} сек.");
    }

    private void UpdateOrderTimer()
    {
        currentOrderTimer -= Time.deltaTime;
        if (currentOrderTimer <= 0f)
        {
            OrderFailed();
        }
    }

    private void OrderFailed()
    {
        isOrderActive = false;
        activeRecipe = null;
        OnOrderExpired?.Invoke();

        Debug.Log("[OrderManager] Заказ просрочен! Жду паузу перед следующим.");
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }

    public bool IsOrderActive => isOrderActive;
    public float GetCurrentOrderRemainingTime() => isOrderActive ? currentOrderTimer : 0f;
}