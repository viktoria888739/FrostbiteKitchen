using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite redSprite;
    
    [Range(0f, 1f)]
    [SerializeField] private float redThreshold = 0.3f;

    [Header("Для теста в Инспекторе")]
    [Range(0f, 1f)]
    [SerializeField] private float testProgress = 1f;

    private float maxTime;
    private float currentTime;
    private bool isRunning;

    private void OnValidate()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = testProgress;
            fillImage.color = Color.white;

            if (testProgress > redThreshold)
            {
                fillImage.sprite = greenSprite;
            }
            else
            {
                fillImage.sprite = redSprite;
            }
        }
    }

    public void StartTimer(float duration)
    {
        maxTime = duration;
        currentTime = duration;
        isRunning = true;
        
        fillImage.color = Color.white; 
        fillImage.sprite = greenSprite;
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        float progress = currentTime / maxTime;
        fillImage.fillAmount = progress;
        
        if (progress > redThreshold)
        {
            fillImage.sprite = greenSprite;
        }
        else
        {
            fillImage.sprite = redSprite;
        }
    }
}