using UnityEngine;
using TMPro;

public class GameOverDisplay : MonoBehaviour
{
    [Header("Крупный вердикт (Победа/Проигрыш)")]
    [SerializeField] private TextMeshProUGUI outcomeText;

    [Header("Текстовые поля статистики")]
    [SerializeField] private TextMeshProUGUI successfulDishesText;
    [SerializeField] private TextMeshProUGUI spoiledDishesText;
    [SerializeField] private TextMeshProUGUI threatsDefendedText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;

    private void OnEnable()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (SessionStatistics.Instance == null)
        {
            Debug.LogWarning("[UI GAME OVER] SessionStatistics.Instance не найден!");
            return;
        }

        SessionData stats = SessionStatistics.Instance.GetSessionData();

        if (outcomeText != null)
            outcomeText.text = ResolveOutcomeText();

        FillStats(stats);
    }

    private string ResolveOutcomeText()
    {
        if (SessionResultEvaluator.Instance == null)
            return "GAME OVER";

        return SessionResultEvaluator.Instance.CurrentResult switch
        {
            SessionResultEvaluator.SessionStatus.Success => "WIN",
            SessionResultEvaluator.SessionStatus.GameOverByMonster => "GAME OVER",
            _ => "GAME OVER"
        };
    }

    private void FillStats(SessionData stats)
    {
        if (successfulDishesText != null)
            successfulDishesText.text = $"{stats.completedOrders}";

        if (spoiledDishesText != null)
            spoiledDishesText.text = $"{stats.failedOrders}";

        if (threatsDefendedText != null)
            threatsDefendedText.text = $"{stats.threatsDefended}";

        if (survivalTimeText != null)
            survivalTimeText.text = $"{stats.totalTime:F1} сек.";
    }
}
