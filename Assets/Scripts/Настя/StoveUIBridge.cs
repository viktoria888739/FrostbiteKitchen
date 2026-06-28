using UnityEngine;
using UnityEngine.UI;

public class StoveUIBridge : MonoBehaviour
{
    [SerializeField] private Stove targetStove;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject barRoot;

    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;

    private void Awake()
    {
        if (targetStove == null)
            targetStove = GetComponentInParent<Stove>();

        if (fillImage == null)
        {
            Transform fillTransform = transform.Find("Fill");
            if (fillTransform != null)
                fillImage = fillTransform.GetComponent<Image>();
        }

        if (barRoot == null)
            barRoot = gameObject;

        SetBarVisible(false);
        ResetBarVisual();
    }

    private void OnEnable()
    {
        if (targetStove == null)
            return;

        targetStove.OnProgressUpdated += UpdateProgressBar;
        targetStove.OnCookingStarted += ShowBar;
        targetStove.OnCookingFinished += OnCookingFinished;
        targetStove.OnDishBurned += HideBar;
    }

    private void OnDisable()
    {
        if (targetStove == null)
            return;

        targetStove.OnProgressUpdated -= UpdateProgressBar;
        targetStove.OnCookingStarted -= ShowBar;
        targetStove.OnCookingFinished -= OnCookingFinished;
        targetStove.OnDishBurned -= HideBar;
    }

    private void ShowBar()
    {
        SetBarVisible(true);
        ResetBarVisual();
    }

    private void OnCookingFinished()
    {
        UpdateProgressBar(1f);
        SetBarVisible(false);
        ResetBarVisual();
    }

    private void HideBar()
    {
        SetBarVisible(false);
        ResetBarVisual();
    }

    private void UpdateProgressBar(float progress)
    {
        if (fillImage == null)
            return;

        float remaining = 1f - Mathf.Clamp01(progress);
        fillImage.fillAmount = remaining;

        if (remaining <= 0f)
        {
            SetBarVisible(false);
            return;
        }

        SetBarVisible(true);

        if (remaining > 0.5f)
            fillImage.color = Color.Lerp(yellowColor, greenColor, (remaining - 0.5f) * 2f);
        else
            fillImage.color = Color.Lerp(redColor, yellowColor, remaining * 2f);
    }

    private void ResetBarVisual()
    {
        if (fillImage == null)
            return;

        fillImage.fillAmount = 1f;
        fillImage.color = greenColor;
    }

    private void SetBarVisible(bool visible)
    {
        if (barRoot != null)
            barRoot.SetActive(visible);
    }
}
