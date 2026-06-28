using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;

    private float maxTime;
    private float currentTime;
    private bool isRunning;
    

    public void StartTimer(float duration)
    {
        maxTime = duration;
        currentTime = duration;
        isRunning = true;
        UpdateUI();
    }

    public void SetRemainingNormalized(float remaining01)
    {
        maxTime = 1f;
        currentTime = Mathf.Clamp01(remaining01);
        isRunning = false;
        UpdateUI();
    }

    public void StopTimer()
    {
        isRunning = false;
        currentTime = 0f;
        UpdateUI();
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
        if (fillImage == null)
            return;

        float progress = maxTime > 0f ? currentTime / maxTime : 0f;
        fillImage.fillAmount = progress;
        
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