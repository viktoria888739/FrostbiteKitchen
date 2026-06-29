using UnityEngine;
using UnityEngine.UI;

public class VisualWarningManager : MonoBehaviour
{
    [SerializeField] private Image warningImage;
    [SerializeField] private float blinkSpeed = 3f;
    [SerializeField] private float hiddenAlpha = 0f;
    [SerializeField] private float blinkMinAlpha = 0.25f;
    [SerializeField] private float blinkMaxAlpha = 1f;

    private bool isWarningActive;
    private bool isPlayerFacingThreat;
    private float blinkTimer;

    private void Awake()
    {
        if (warningImage == null)
            warningImage = GetComponent<Image>();

        HideWarning();
    }

    private void OnEnable()
    {
        ThreatManager.OnThreatStarted += HandleThreatStarted;
        ThreatManager.OnThreatCleared += HideWarning;
        ThreatManager.OnThreatFailed += HideWarning;
    }

    private void OnDisable()
    {
        ThreatManager.OnThreatStarted -= HandleThreatStarted;
        ThreatManager.OnThreatCleared -= HideWarning;
        ThreatManager.OnThreatFailed -= HideWarning;
    }

    private void Update()
    {
        if (!isWarningActive || warningImage == null)
            return;

        if (isPlayerFacingThreat)
        {
            SetAlpha(hiddenAlpha);
            return;
        }

        blinkTimer += Time.deltaTime * blinkSpeed;
        float alpha = Mathf.Lerp(blinkMinAlpha, blinkMaxAlpha, (Mathf.Sin(blinkTimer) + 1f) * 0.5f);
        SetAlpha(alpha);
    }

    public void UpdatePlayerView(KitchenSide currentSide)
    {
        if (!isWarningActive || ThreatManager.Instance == null)
            return;

        isPlayerFacingThreat = ThreatManager.Instance.ActiveSide == currentSide;
        blinkTimer = 0f;
    }

    private void HandleThreatStarted(KitchenSide side)
    {
        ShowWarning(side);

        KitchenRotation rotation = Object.FindFirstObjectByType<KitchenRotation>();
        if (rotation != null)
            UpdatePlayerView(rotation.CurrentSide);
    }

    public void ShowWarning(KitchenSide side)
    {
        isWarningActive = true;
        isPlayerFacingThreat = false;
        blinkTimer = 0f;

        if (warningImage != null)
            warningImage.gameObject.SetActive(true);
    }

    public void HideWarning()
    {
        isWarningActive = false;
        isPlayerFacingThreat = false;
        blinkTimer = 0f;

        if (warningImage != null)
        {
            SetAlpha(hiddenAlpha);
            warningImage.gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (warningImage == null)
            return;

        Color color = warningImage.color;
        color.a = alpha;
        warningImage.color = color;
    }
}
