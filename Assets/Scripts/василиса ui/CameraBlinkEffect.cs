using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CameraBlinkEffect : MonoBehaviour
{
    public static CameraBlinkEffect Instance { get; private set; }
    [Header("Настройки затемнения")]
    [SerializeField] private float fadeOutDuration = 0.2f;

    private CanvasGroup canvasGroup;

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

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false; 
    }
    
    public void PlayBlink()
    {
        StopAllCoroutines();
        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        canvasGroup.alpha = 1f;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeOutDuration);
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
    }
}