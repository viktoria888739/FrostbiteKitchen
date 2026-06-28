using UnityEngine;
using FrostbiteKitchen.Data;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Настройки заказов")]
    [SerializeField] private RecipeCatalog recipeCatalog;
    [Tooltip("Время между появлением новых заказов (в секундах)")]
    [SerializeField] private float timeBetweenOrders = 9f;

    [Header("Текущий заказ")]
    [SerializeField] private RecipeData activeRecipe;
    private float currentOrderTimer;
    private bool isOrderActive = false;
    public static System.Action OnOrderSubmitted;
    public static System.Action OnOrderExpired;
    public static System.Action<RecipeData> OnNewOrderStarted;

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
        Debug.Log($"[OrderManager] Новый заказ: {activeRecipe.recipeName} | Время: {currentOrderTimer:F1}с");
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
        SessionStatistics.Instance?.AddFailedOrder();
        OnOrderExpired?.Invoke();
        SessionOrderTracker.Instance?.RegisterCompletedOrder();
        Debug.Log("[OrderManager] Заказ провален по времени!");
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }

    public void CompleteActiveOrder()
    {
        if (!isOrderActive) return;
        Debug.Log($"[OrderManager] Заказ {activeRecipe.recipeName} успешно сдан!");
        SessionStatistics.Instance?.AddCompletedOrder();
        OnOrderSubmitted?.Invoke();
        SessionOrderTracker.Instance?.RegisterCompletedOrder();
        isOrderActive = false;
        activeRecipe = null;
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }
    public void FailCurrentOrder()
    {
        if (!isOrderActive) return;
        OrderFailed();
    }
}