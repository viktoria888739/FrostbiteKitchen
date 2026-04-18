using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private List<RecipeData> allRecipes; // Список всех рецептов из папки Assets
    [SerializeField] private float timeBetweenOrders = 5f; // Пауза перед новым заказом

    [Header("Current Order Info")]
    [SerializeField] private RecipeData activeRecipe;
    private float currentOrderTimer;
    private bool isOrderActive = false;

    // События для Василисы (UI) и Анастасии (Звуки/Эффекты)
    public static System.Action<RecipeData> OnNewOrderStarted;
    public static System.Action OnOrderExpired;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Заказы работают только когда состояние игры "Gameplay"
        if (GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay) return;

        if (isOrderActive)
        {
            UpdateOrderTimer();
        }
        else
        {
            // Если заказа нет, можно реализовать логику ожидания следующего
            // Для прототипа пока просто запускаем первый заказ
            if (activeRecipe == null) StartNewRandomOrder();
        }
    }

    public void StartNewRandomOrder()
    {
        if (allRecipes.Count == 0) return;

        // Выбираем случайный рецепт из списка
        activeRecipe = allRecipes[Random.Range(0, allRecipes.Count)];
        currentOrderTimer = activeRecipe.timeLimit;
        isOrderActive = true;

        // Оповещаем всех (UI обновит текст и картинку блюда)
        OnNewOrderStarted?.Invoke(activeRecipe);

        Debug.Log($"[OrderManager] Новый заказ: {activeRecipe.displayName}");
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

        // Через небольшую паузу запускаем новый заказ
        Invoke(nameof(StartNewRandomOrder), timeBetweenOrders);
    }
}