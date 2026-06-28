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
        
        float successPercentage = 0f;
        if (stats.totalOrdersAttempted > 0)
        {
            successPercentage = ((float)stats.successfulDishes / stats.totalOrdersAttempted) * 100f;
        }
        
        float requiredPercentage = 50f;
        if (SettingsLoader.Instance != null && SettingsLoader.Instance.CurrentSettings != null)
        {
            requiredPercentage = SettingsLoader.Instance.CurrentSettings.winRequiredOrderPercentage;
        }
        
        if (outcomeText != null)
        {
            if (successPercentage < requiredPercentage)
            {
                outcomeText.text = "GAME OVER";
            }
            else
            {
                outcomeText.text = "WIN";
            }
        }
        
        if (successfulDishesText != null)
        {
            successfulDishesText.text = $"{stats.successfulDishes}";
        }

        if (spoiledDishesText != null)
        {
            spoiledDishesText.text = $"{stats.spoiledDishes}";
        }

        if (threatsDefendedText != null)
        {
            threatsDefendedText.text = $"{stats.threatsDefended}";
        }

        if (survivalTimeText != null)
        {
            survivalTimeText.text = $"{stats.survivalTime:F1} сек.";
        }
    }
}