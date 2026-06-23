using UnityEngine;
using UnityEngine.UI;

public class StoveUIBridge : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Stove targetStove;
    [SerializeField] private Image fillImage;

    [Header("Цвета из префаба Василисы")]
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;

    private void OnEnable()
    {
        if (targetStove != null)
        {
            // Подписываемся на событие жарки плиты
            targetStove.OnProgressUpdated += UpdateProgressBar;
        }
    }

    private void OnDisable()
    {
        if (targetStove != null)
        {
            targetStove.OnProgressUpdated -= UpdateProgressBar;
        }
    }

    private void UpdateProgressBar(float current, float max)
    {
        if (fillImage == null || max <= 0) return;

        // Считаем прогресс от 0 до 1
        float progress = current / max;
        fillImage.fillAmount = progress;

        // Меняем цвет полоски (в точности как логика Василисы, но в сторону увеличения)
        if (progress > 0.5f)
        {
            fillImage.color = Color.Lerp(yellowColor, greenColor, (progress - 0.5f) * 2f);
        }
        else
        {
            fillImage.color = Color.Lerp(redColor, yellowColor, progress * 2f);
        }
    }
}