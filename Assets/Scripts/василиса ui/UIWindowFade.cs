using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindowFade : MonoBehaviour
{
    [Header("Настройки анимации")]
    [SerializeField] private float fadeDuration = 0.25f;

    private CanvasGroup canvasGroup;
    
    private bool skipNextFade = false; 

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (skipNextFade)
        {
            canvasGroup.alpha = 1f;
            skipNextFade = false;
            return;
        }
        
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeInCoroutine());
    }

    public void SkipFadeNextTime()
    {
        skipNextFade = true;
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            
            yield return null; 
        }

        canvasGroup.alpha = 1f;
    }
}