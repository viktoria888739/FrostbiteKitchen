using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Gameplay;

public class FlashlightToggle : MonoBehaviour, IInteractable
{
    [SerializeField] private LeftThreatHandler leftThreatHandler;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activeSprite;

    private int lastInteractFrame = -1;
    private bool lastVisualState;

    private void Awake()
    {
        ResolveIconImage();
    }

    private void Start()
    {
        ApplyVisual(force: true);
    }

    private void Update()
    {
        LeftThreatHandler handler = ResolveHandler();
        bool isOn = handler != null && handler.IsFlashlightOn;
        if (isOn != lastVisualState)
            ApplyVisual(force: true);
    }

    public void Interact()
    {
        if (Time.frameCount == lastInteractFrame)
            return;

        lastInteractFrame = Time.frameCount;
        ToggleFlashlight();
    }

    public void ToggleFlashlight()
    {
        LeftThreatHandler handler = ResolveHandler();
        if (handler != null)
            handler.ToggleFlashlight();
        else
            Debug.LogWarning("[ФОНАРИК] LeftThreatHandler не найден на сцене.");

        ApplyVisual(force: true);
    }

    private void ResolveIconImage()
    {
        if (iconImage != null)
            return;

        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject == gameObject)
                continue;

            iconImage = image;
            return;
        }
    }

    private void ApplyVisual(bool force = false)
    {
        if (iconImage == null)
            return;

        LeftThreatHandler handler = ResolveHandler();
        bool isOn = handler != null && handler.IsFlashlightOn;

        if (!force && isOn == lastVisualState)
            return;

        lastVisualState = isOn;
        Sprite targetSprite = isOn ? activeSprite : defaultSprite;
        if (targetSprite != null)
            iconImage.sprite = targetSprite;
    }

    private LeftThreatHandler ResolveHandler()
    {
        if (leftThreatHandler != null)
            return leftThreatHandler;

        if (LeftThreatHandler.Instance != null)
            return LeftThreatHandler.Instance;

        return Object.FindFirstObjectByType<LeftThreatHandler>(FindObjectsInactive.Include);
    }
}
