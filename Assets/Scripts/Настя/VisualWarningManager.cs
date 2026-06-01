using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VisualWarningManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Image warningImage; // Ссылка на ваш UI Image
    [SerializeField] private float blinkSpeed = 5f; // Скорость мигания (сделаем быстрее для опасности)
    [SerializeField] private float steadyAlpha = 0.4f; // Прозрачность индикатора, когда он просто горит (от 0 до 1)

    private bool hasActiveThreat = false;
    private KitchenSide currentThreatLocation;
    private KitchenSide currentPlayerView;

    private Coroutine blinkCoroutine;
    private Color originalColor;

    private void Start()
    {
        if (warningImage != null)
        {
            originalColor = warningImage.color;
            warningImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Включает предупреждение и запоминает, где монстр
    /// </summary>
    public void ShowWarning(KitchenSide threatLocation)
    {
        hasActiveThreat = true;
        currentThreatLocation = threatLocation;

        UpdateWarningState();
    }

    /// <summary>
    /// Выключает предупреждение
    /// </summary>
    public void HideWarning()
    {
        hasActiveThreat = false;

        if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);

        if (warningImage != null)
        {
            warningImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Этот метод нужно вызывать из вашего скрипта поворота камер, когда игрок меняет обзор!
    /// </summary>
    public void UpdatePlayerView(KitchenSide newView)
    {
        currentPlayerView = newView;

        // Если в игре сейчас есть монстр, проверяем состояние заново при каждом повороте
        if (hasActiveThreat)
        {
            UpdateWarningState();
        }
    }

    /// <summary>
    /// Проверяет, совпадает ли взгляд игрока с монстром, и выбирает режим (мигание или покой)
    /// </summary>
    private void UpdateWarningState()
    {
        if (warningImage == null) return;

        warningImage.gameObject.SetActive(true);

        // Если игрок смотрит ТУДА ЖЕ, где монстр — запускаем мигание
        if (currentPlayerView == currentThreatLocation)
        {
            if (blinkCoroutine == null)
            {
                blinkCoroutine = StartCoroutine(BlinkRoutine());
            }
        }
        else // Если монстр на другой стороне — индикатор просто горит
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }
            // Устанавливаем фиксированную невысокую прозрачность
            warningImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, steadyAlpha);
        }
    }

    /// <summary>
    /// Корутина мигания
    /// </summary>
    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            warningImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}