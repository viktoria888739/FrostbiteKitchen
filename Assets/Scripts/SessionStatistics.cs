using UnityEngine;

public class SessionStatistics : MonoBehaviour
{
    public static SessionStatistics Instance { get; private set; }

    public int successfulDishes = 0;
    public int spoiledDishes = 0;
    public int threatsDefended = 0;
    public float timeSurvived = 0f;

    public int completedOrders = 0;
    public int failedOrders = 0;
    public float sessionTime = 0f;

    private bool isSessionActive = false;

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

    private void Update()
    {
        if (isSessionActive)
        {
            sessionTime += Time.deltaTime;
        }
    }

    public void StartSession()
    {
        ResetStatistics();
        isSessionActive = true;
        Debug.Log("[SessionStatistics] Сессия начата — статистика сброшена");
    }

    public void EndSession()
    {
        isSessionActive = false;
        timeSurvived = sessionTime;
        Debug.Log($"[SessionStatistics] Сессия завершена. Выполнено: {completedOrders} | Провалено: {failedOrders} | Время: {sessionTime:F1}с");
    }

    public void AddCompletedOrder()
    {
        completedOrders++;
        successfulDishes++;
        Debug.Log($"[SessionStatistics] ✅ Заказ выполнен! Всего: {completedOrders}");
    }

    public void AddFailedOrder()
    {
        failedOrders++;
        spoiledDishes++;
        Debug.Log($"[SessionStatistics] ❌ Заказ провален! Всего: {failedOrders}");
    }

    public SessionData GetSessionData()
    {
        return new SessionData
        {
            successfulDishes = this.successfulDishes,
            spoiledDishes = this.spoiledDishes,
            threatsDefended = this.threatsDefended,
            survivalTime = this.sessionTime,
            completedOrders = this.completedOrders,
            failedOrders = this.failedOrders,
            totalTime = this.sessionTime,
            totalOrdersAttempted = this.completedOrders + this.failedOrders
        };
    }

    public void AddSuccessfulDish() => successfulDishes++;
    public void AddSpoiledDish() => spoiledDishes++;
    public void AddDefendedThreat() => threatsDefended++;

    public void ResetStatistics()
    {
        successfulDishes = spoiledDishes = threatsDefended = 0;
        timeSurvived = 0f;
        completedOrders = failedOrders = 0;
        sessionTime = 0f;
    }
}

[System.Serializable]
public struct SessionData
{
    public int successfulDishes;
    public int spoiledDishes;
    public int threatsDefended;
    public float survivalTime;
    public int totalOrdersAttempted;

    public int completedOrders;
    public int failedOrders;
    public float totalTime;
}