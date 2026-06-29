using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Gameplay;

public class LeftThreatHandler : MonoBehaviour, IInteractable
{
    public static LeftThreatHandler Instance { get; private set; }

    [Header("Настройки фонарика")]
    [SerializeField] private GameObject flashlightSpotlight;
    [SerializeField] private Image ventilationImage;
    [SerializeField] private Sprite normalVentSprite;
    [SerializeField] private Sprite litVentSprite;
    [SerializeField] private Sprite monsterVentSprite;

    private bool isMonsterPresent;
    private bool isFlashlightOn;
    private int lastInteractFrame = -1;

    public bool IsFlashlightOn => isFlashlightOn;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    private void ResolveReferences()
    {
        if (flashlightSpotlight == null)
        {
            Transform spotlight = transform.Find("Light_Spotlight");
            if (spotlight != null)
                flashlightSpotlight = spotlight.gameObject;
        }

        if (ventilationImage == null)
            ventilationImage = GetComponent<Image>();

        if (normalVentSprite == null && ventilationImage != null)
            normalVentSprite = ventilationImage.sprite;
    }

    public void Interact()
    {
        ToggleFlashlight();
    }

    public void OnMonsterSpawn()
    {
        isMonsterPresent = true;

        if (!isFlashlightOn && monsterVentSprite != null)
            SetVentSprite(monsterVentSprite);
    }

    public void ToggleFlashlight()
    {
        if (Time.frameCount == lastInteractFrame)
            return;

        lastInteractFrame = Time.frameCount;
        ResolveReferences();

        if (isFlashlightOn)
        {
            SetFlashlightState(false);
            Debug.Log("<color=white>[ФОНАРИК] Свет выключен</color>");
            return;
        }

        SetFlashlightState(true);
        GameAudioManager.Instance?.PlayFlashlight();
        Debug.Log("<color=yellow>[ФОНАРИК] Свет включён</color>");

        if (isMonsterPresent && ThreatManager.Instance != null && ThreatManager.Instance.IsActiveThreatOn(KitchenSide.Left))
            ResolveThreat();
    }

    private void SetFlashlightState(bool on)
    {
        if (isFlashlightOn == on)
            return;

        if (on)
        {
            isFlashlightOn = true;
            ViewRotationBlocker.PushBlock();
        }
        else
        {
            isFlashlightOn = false;
            ViewRotationBlocker.PopBlock();
        }

        ApplyFlashlightVisual(isFlashlightOn);
    }

    private void ApplyFlashlightVisual(bool on)
    {
        if (on)
        {
            if (litVentSprite != null)
                SetVentSprite(litVentSprite);
        }
        else if (isMonsterPresent && monsterVentSprite != null)
        {
            SetVentSprite(monsterVentSprite);
        }
        else if (normalVentSprite != null)
        {
            SetVentSprite(normalVentSprite);
        }

        if (flashlightSpotlight != null)
            flashlightSpotlight.SetActive(on);
    }

    private void ResolveThreat()
    {
        isMonsterPresent = false;

        if (isFlashlightOn && litVentSprite != null)
            SetVentSprite(litVentSprite);
        else if (normalVentSprite != null)
            SetVentSprite(normalVentSprite);

        ThreatManager.Instance?.PlayerDefendedThreat(KitchenSide.Left);
    }

    private void SetVentSprite(Sprite sprite)
    {
        if (ventilationImage != null)
            ventilationImage.sprite = sprite;
    }

    public void OnPlayerTurnAway()
    {
        if (!isFlashlightOn)
            return;

        SetFlashlightState(false);
    }

    public void ResetThreat()
    {
        isMonsterPresent = false;

        if (isFlashlightOn)
            SetFlashlightState(false);
        else
            ApplyFlashlightVisual(false);

        if (normalVentSprite != null)
            SetVentSprite(normalVentSprite);
    }
}
