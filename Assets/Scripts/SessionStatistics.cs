using UnityEngine;

public class SessionStatistics : MonoBehaviour
{
    public static SessionStatistics Instance { get; private set; }

    [Header("Статистика сессии")]
    public int successfulDishes = 0;
    public int spoiledDishes = 0;
    public int threatsDefended = 0;
    public float timeSurvived = 0f;

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

    public void AddSuccessfulDish() => successfulDishes++;
    public void AddSpoiledDish() => spoiledDishes++;
    public void AddDefendedThreat() => threatsDefended++;

    public void ResetStatistics()
    {
        successfulDishes = spoiledDishes = threatsDefended = 0;
        timeSurvived = 0f;
    }

    public SessionData GetSessionData()
    {
        return new SessionData
        {
            successfulDishes = successfulDishes,
            spoiledDishes = spoiledDishes,
            threatsDefended = threatsDefended,
            survivalTime = Time.timeSinceLevelLoad,
            totalOrdersAttempted = successfulDishes + spoiledDishes
        };
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
}