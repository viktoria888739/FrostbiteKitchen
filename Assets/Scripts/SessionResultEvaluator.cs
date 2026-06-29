using UnityEngine;
using FrostbiteKitchen.Data;

public class SessionResultEvaluator : MonoBehaviour
{
    public static SessionResultEvaluator Instance { get; private set; }

    public enum SessionStatus
    {
        Success,
        Fail,
        GameOverByMonster
    }

    public SessionStatus CurrentResult { get; private set; } = SessionStatus.Fail;

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

    private void OnEnable()
    {
        OrderManager.OnNewOrderStarted += OnNewOrderGenerated;
    }

    private void OnDisable()
    {
        OrderManager.OnNewOrderStarted -= OnNewOrderGenerated;
    }

    private void OnNewOrderGenerated(RecipeData recipe)
    {
    }
    public void HandleThreatMissed()
    {
        CurrentResult = SessionStatus.GameOverByMonster;
        Debug.LogError("[SessionResultEvaluator] 💀 ИГРОК УБИТ МОНСТРОМ. Кулинарные расчеты прекращены!");

        OrderManager.Instance?.StopManager();

        if (GameStateMachine.Instance != null)
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Screamer);
    }
    public void EvaluateSessionResult()
    {
        if (CurrentResult == SessionStatus.GameOverByMonster)
        {
            GameAudioManager.Instance?.PlaySessionGameOver();
            return;
        }

        int completed = SessionStatistics.Instance != null ? SessionStatistics.Instance.completedOrders : 0;
        int failed = SessionStatistics.Instance != null ? SessionStatistics.Instance.failedOrders : 0;
        int total = completed + failed;

        if (total <= 0)
        {
            CurrentResult = SessionStatus.Fail;
            Debug.LogWarning("[SessionResultEvaluator] Заказов не было. Технический провал смены.");
            return;
        }

        float successRate = ((float)completed / total) * 100f;

        float requiredPercentage = 50f;
        if (SettingsLoader.Instance != null && SettingsLoader.Instance.CurrentSettings != null)
            requiredPercentage = SettingsLoader.Instance.CurrentSettings.winRequiredOrderPercentage;

        if (successRate >= requiredPercentage)
        {
            CurrentResult = SessionStatus.Success;
            Debug.Log($"[SessionResultEvaluator] Смена сдана! Успех: {successRate:F1}%");
            GameAudioManager.Instance?.PlaySessionWin();
        }
        else
        {
            CurrentResult = SessionStatus.Fail;
            Debug.Log($"[SessionResultEvaluator] Смена провалена. Успех всего: {successRate:F1}%");
            GameAudioManager.Instance?.PlaySessionGameOver();
        }
    }
    public void ResetStatistics()
    {
        CurrentResult = SessionStatus.Fail;
        Debug.Log("[SessionResultEvaluator] Статус сессии сброшен для новой игры.");
    }
}