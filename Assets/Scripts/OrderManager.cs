using UnityEngine;
using FrostbiteKitchen.Data;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

<<<<<<< Updated upstream
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
=======
    public static event Action<RecipeData> OnNewOrderStarted;
    public static event Action OnOrderExpired;
    public static event Action OnOrderSubmitted;
    public static event Action OnOrderFailed;

    [SerializeField] private RecipeCatalog recipeCatalog;
    [SerializeField] private List<RecipeData> availableRecipes;
    [SerializeField] private float orderSpawnInterval = 15f;
    [SerializeField] private float delayBeforeNextOrder = 2f;

    private readonly List<RecipeData> runtimeRecipes = new List<RecipeData>();

    private RecipeData activeRecipe;
    private float timeRemaining;
    private bool isManagerActive = true;
    private bool hasActiveOrder;
    private Coroutine spawnRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildRuntimeRecipes();
    }

    private void BuildRuntimeRecipes()
    {
        runtimeRecipes.Clear();

        if (availableRecipes != null)
        {
            foreach (RecipeData recipe in availableRecipes)
            {
                if (recipe != null && !runtimeRecipes.Contains(recipe))
                    runtimeRecipes.Add(recipe);
            }
        }

        if (runtimeRecipes.Count == 0 && recipeCatalog != null && recipeCatalog.AllRecipes != null)
        {
            foreach (RecipeData recipe in recipeCatalog.AllRecipes)
            {
                if (recipe != null && !runtimeRecipes.Contains(recipe))
                    runtimeRecipes.Add(recipe);
            }
        }

        if (runtimeRecipes.Count == 0)
            Debug.LogError("[OrderManager] Нет рецептов.");
    }

    private void Start()
    {
        if (isManagerActive)
        {
            spawnRoutine = StartCoroutine(OrderSpawnRoutine());
        }
>>>>>>> Stashed changes
    }

    private void Update()
    {
<<<<<<< Updated upstream
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
=======
        if (!isManagerActive || !hasActiveOrder) return;

        if (GameStateMachine.Instance != null &&
            GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            FailCurrentOrder();
        }
    }

    public RecipeData GetActiveRecipe()
    {
        return hasActiveOrder ? activeRecipe : null;
    }

    public void CompleteActiveOrder()
    {
        if (!hasActiveOrder) return;
        FinishOrder(success: true);
    }

    public void FailCurrentOrder()
    {
        if (!hasActiveOrder) return;
        FinishOrder(success: false, wrongDish: false);
    }

    public void FailWrongOrder()
    {
        if (!hasActiveOrder) return;
        FinishOrder(success: false, wrongDish: true);
    }

    private IEnumerator OrderSpawnRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (isManagerActive)
        {
            if (CanSpawnOrder())
            {
                SpawnRandomOrder();
            }

            yield return new WaitForSeconds(orderSpawnInterval);
        }
    }

    private bool CanSpawnOrder()
    {
        if (!isManagerActive || hasActiveOrder) return false;

        if (SessionOrderTracker.Instance != null &&
            SessionOrderTracker.Instance.ProcessedCustomersCount >= SessionOrderTracker.Instance.MaxOrders)
        {
            return false;
        }

        if (GameStateMachine.Instance != null &&
            GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
        {
            return false;
        }

        return runtimeRecipes.Count > 0;
    }

    private void SpawnRandomOrder()
    {
        if (runtimeRecipes.Count == 0)
            return;

        activeRecipe = runtimeRecipes[UnityEngine.Random.Range(0, runtimeRecipes.Count)];
        timeRemaining = activeRecipe.timeLimit > 0f ? activeRecipe.timeLimit : 60f;
        hasActiveOrder = true;

        GameAudioManager.Instance?.PlayOrderNew();
        OnNewOrderStarted?.Invoke(activeRecipe);
    }

    private void FinishOrder(bool success, bool wrongDish = false)
    {
        hasActiveOrder = false;
        activeRecipe = null;

        if (success)
        {
            SessionStatistics.Instance?.AddCompletedOrder();
            GameAudioManager.Instance?.PlayOrderSuccess();
            OnOrderSubmitted?.Invoke();
            SessionOrderTracker.Instance?.RegisterSuccessfulOrder();
        }
        else
        {
            SessionStatistics.Instance?.AddFailedOrder();

            if (wrongDish)
                GameAudioManager.Instance?.PlayOrderWrong();
            else
                GameAudioManager.Instance?.PlayOrderExpired();

            OnOrderExpired?.Invoke();
            OnOrderFailed?.Invoke();
            SessionOrderTracker.Instance?.RegisterFailedOrder();
        }

        if (isManagerActive && delayBeforeNextOrder > 0f)
        {
            StartCoroutine(SpawnNextOrderAfterDelay());
        }
    }

    private IEnumerator SpawnNextOrderAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextOrder);

        if (CanSpawnOrder())
        {
            SpawnRandomOrder();
        }
    }

    public void StopManager()
    {
        isManagerActive = false;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        hasActiveOrder = false;
        activeRecipe = null;
        StopAllCoroutines();
>>>>>>> Stashed changes
    }
}
