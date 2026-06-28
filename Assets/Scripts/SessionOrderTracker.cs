using UnityEngine;
using System;

public class SessionOrderTracker : MonoBehaviour
{
    public static SessionOrderTracker Instance { get; private set; }

    public static event Action OnSessionCompleted;
    public static event Action<int, int> OnCustomerCountChanged;

    [SerializeField] private int maxOrders = 10;

    private int servedCustomersCount = 0;
    private int processedCustomersCount = 0;

    public int CurrentOrdersCount => servedCustomersCount;
    public int ProcessedCustomersCount => processedCustomersCount;
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

    private void Start()
    {
        NotifyCountChanged();
    }

    public void RegisterSuccessfulOrder()
    {
        if (isSessionEnded) return;

        servedCustomersCount++;
        processedCustomersCount++;
        NotifyCountChanged();
        TryEndSession();
    }

    public void RegisterFailedOrder()
    {
        if (isSessionEnded) return;

        processedCustomersCount++;
        NotifyCountChanged();
        TryEndSession();
    }

    private void TryEndSession()
    {
        if (processedCustomersCount >= maxOrders)
        {
            EndSession();
        }
    }

    private void EndSession()
    {
        if (isSessionEnded) return;
        isSessionEnded = true;

        OrderManager.Instance?.StopManager();

        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Results);
        }

        OnSessionCompleted?.Invoke();
    }

    public void ResetSession()
    {
        servedCustomersCount = 0;
        processedCustomersCount = 0;
        isSessionEnded = false;
        NotifyCountChanged();
    }

    private void NotifyCountChanged()
    {
        OnCustomerCountChanged?.Invoke(servedCustomersCount, maxOrders);
    }
}
