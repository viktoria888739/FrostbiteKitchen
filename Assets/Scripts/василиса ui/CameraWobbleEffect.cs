using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CameraWobbleEffect : MonoBehaviour
{
    public static CameraWobbleEffect Instance { get; private set; }

    [Header("Настройки поворота головы")]
    [Tooltip("Сила инерции (на сколько пикселей сдвигается картинка)")]
    [SerializeField] private float slideAmount = 45f;

    [Tooltip("Общая длительность эффекта")]
    [SerializeField] private float slideDuration = 0.5f;

    [Tooltip("Доля времени на быстрый сдвиг в сторону поворота (остальное — плавное возвращение)")]
    [Range(0.2f, 0.5f)]
    [SerializeField] private float pushPhaseRatio = 0.35f;

    [Tooltip("Время сглаживания при возврате в исходное положение")]
    [SerializeField] private float settleSmoothTime = 0.18f;

    private RectTransform rectTransform;
    private Vector2 initialPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    public void PlayWobble(int direction)
    {
        StopAllCoroutines();
        StartCoroutine(HeadTurnCoroutine(direction));
    }

    private IEnumerator HeadTurnCoroutine(int direction)
    {
        float targetX = -slideAmount * direction;
        float pushDuration = slideDuration * pushPhaseRatio;
        float settleDuration = slideDuration - pushDuration;

        float elapsedTime = 0f;
        while (elapsedTime < pushDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / pushDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float currentOffsetX = targetX * eased;
            rectTransform.anchoredPosition = new Vector2(initialPosition.x + currentOffsetX, initialPosition.y);
            yield return null;
        }

        Vector2 velocity = Vector2.zero;
        Vector2 currentPosition = rectTransform.anchoredPosition;
        elapsedTime = 0f;

        while (elapsedTime < settleDuration)
        {
            elapsedTime += Time.deltaTime;
            currentPosition = Vector2.SmoothDamp(
                currentPosition,
                initialPosition,
                ref velocity,
                settleSmoothTime,
                Mathf.Infinity,
                Time.deltaTime);

            rectTransform.anchoredPosition = currentPosition;

            if ((currentPosition - initialPosition).sqrMagnitude < 0.01f)
                break;

            yield return null;
        }

        rectTransform.anchoredPosition = initialPosition;
    }
}
