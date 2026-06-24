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
            // Подписываемся на событие прогресса жарки плиты
            targetStove.OnProgressUpdated += UpdateProgressBar;
        }
    }

    private void OnDisable()
    {
        if (targetStove != null)
        {
            // Отписываемся при выключении объекта во избежание утечек памяти
            targetStove.OnProgressUpdated -= UpdateProgressBar;
        }
    }

    private void UpdateProgressBar(float progress)
    {
        if (fillImage == null) return;

        // Задаем заполнение полоски (слайдера)
        fillImage.fillAmount = Mathf.Clamp01(progress);

        // Интерполяция цвета в зависимости от этапа готовки (красный -> желтый -> зеленый)
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