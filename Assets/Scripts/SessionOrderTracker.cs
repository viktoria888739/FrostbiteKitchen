using UnityEngine;
using System;

public class SessionOrderTracker : MonoBehaviour
{
    public static SessionOrderTracker Instance { get; private set; }

    public static event Action OnSessionCompleted;

    [Header("Настройки сессии")]
    [SerializeField] private int maxOrders = 10;

    private int currentOrdersCount = 0;

    public int CurrentOrdersCount => currentOrdersCount;
    public int MaxOrders => maxOrders;

    private bool isSessionEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void RegisterCompletedOrder()
    {
        if (isSessionEnded) return;

        currentOrdersCount++;
        Debug.Log($"<color=#33FFF7>[СЕССИЯ]</color> Заказ завершён! Обслужено: {currentOrdersCount}/{maxOrders}");

        if (currentOrdersCount >= maxOrders)
        {
            EndSession();
        }
    }

    private void EndSession()
    {
        if (isSessionEnded) return;
        isSessionEnded = true;

        Debug.Log("<color=green>[СЕССИЯ] Лимит заказов достигнут! Завершение смены...</color>");

        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Results);
        }

        OnSessionCompleted?.Invoke();
    }

    public void ResetSession()
    {
        currentOrdersCount = 0;
        isSessionEnded = false;
    }
}