using UnityEngine;
using UnityEngine.UI;

public class StoveUIBridge : MonoBehaviour
{
    private const string BackgroundChildName = "Image";
    private const string FillChildName = "Fill";

    [SerializeField] private Stove targetStove;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fillImage;

    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;

    private bool isSubscribed;

    private void Awake()
    {
        RemoveLegacyProgressBarComponent();
        ResolveReferences();
        PrepareFillImage();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        WireToStove(GetComponentInParent<Stove>());
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void WireToStove(Stove stove)
    {
        if (stove == null)
            stove = GetComponentInParent<Stove>();

        if (stove == null)
            return;

        Unsubscribe();
        targetStove = stove;
        TrySubscribe();
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();

        if (transform.parent != null)
            transform.parent.SetAsLastSibling();
    }

    private void RemoveLegacyProgressBarComponent()
    {
        ProgressBar legacyBar = GetComponent<ProgressBar>();
        if (legacyBar == null)
            return;

        if (Application.isPlaying)
            Destroy(legacyBar);
        else
            DestroyImmediate(legacyBar);
    }

    private void ResolveReferences()
    {
        if (targetStove == null)
            targetStove = GetComponentInParent<Stove>();

        if (backgroundImage == null)
        {
            Transform backgroundTransform = transform.Find(BackgroundChildName);
            if (backgroundTransform != null)
                backgroundImage = backgroundTransform.GetComponent<Image>();
        }

        if (fillImage == null)
        {
            Transform fillTransform = transform.Find(FillChildName);
            if (fillTransform != null)
                fillImage = fillTransform.GetComponent<Image>();
        }
    }

    private void PrepareFillImage()
    {
        if (fillImage == null)
            return;

        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;
        fillImage.raycastTarget = false;

        if (backgroundImage != null)
            backgroundImage.raycastTarget = false;
    }

    private void TrySubscribe()
    {
        if (isSubscribed || targetStove == null)
            return;

        targetStove.OnProgressUpdated += UpdateProgressBar;
        targetStove.OnCookingStarted += ShowBar;
        targetStove.OnIngredientBurned += HideBar;
        targetStove.OnStoveCleared += HideBar;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || targetStove == null)
            return;

        targetStove.OnProgressUpdated -= UpdateProgressBar;
        targetStove.OnCookingStarted -= ShowBar;
        targetStove.OnIngredientBurned -= HideBar;
        targetStove.OnStoveCleared -= HideBar;
        isSubscribed = false;
    }

    private void ShowBar()
    {
        ResolveReferences();
        BringToFront();
        gameObject.SetActive(true);
        ResetBarVisual();
    }

    private void HideBar()
    {
        ResetBarVisual();
        gameObject.SetActive(false);
    }

    private void UpdateProgressBar(float progress)
    {
        if (fillImage == null)
            ResolveReferences();

        if (fillImage == null)
            return;

        float elapsed = Mathf.Clamp01(progress);
        float remaining = 1f - elapsed;

        fillImage.fillAmount = remaining;
        gameObject.SetActive(true);
        BringToFront();

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
}
