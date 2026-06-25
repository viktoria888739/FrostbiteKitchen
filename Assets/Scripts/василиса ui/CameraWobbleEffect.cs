using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CameraWobbleEffect : MonoBehaviour
{
    public static CameraWobbleEffect Instance { get; private set; }

    [Header("Настройки поворота головы")]
    [Tooltip("Сила инерции (на сколько пикселей сдвигается картинка)")]
    [SerializeField] private float slideAmount = 50f; 
    
    [Tooltip("Время затухания инерции")]
    [SerializeField] private float slideDuration = 0.35f; 

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
        float elapsedTime = 0f;
        float targetX = -slideAmount * direction; 
        
        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slideDuration;
            
            float currentOffsetX = Mathf.Sin(progress * Mathf.PI) * targetX;
            rectTransform.anchoredPosition = new Vector2(initialPosition.x + currentOffsetX, initialPosition.y);
            yield return null;
        }
        
        rectTransform.anchoredPosition = initialPosition;
    }
}