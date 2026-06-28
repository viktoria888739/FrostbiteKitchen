using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftThreatHandler : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки фонарика")]
    [SerializeField] private GameObject flashlightSpotlight;
    [SerializeField] private Image ventilationImage;
    [SerializeField] private Sprite normalVentSprite;
    [SerializeField] private Sprite litVentSprite;
    [SerializeField] private Sprite monsterVentSprite;

    private bool isMonsterPresent;
    private bool isFlashlightOn;

    private void Awake()
    {
        ResolveReferences();
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

    public void OnPointerClick(PointerEventData eventData)
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
        ResolveReferences();

        isFlashlightOn = !isFlashlightOn;
        ApplyFlashlightVisual(isFlashlightOn);
        ViewRotationBlocker.SetBlock(isFlashlightOn);

        if (isFlashlightOn)
            GameAudioManager.Instance?.PlayFlashlight();

        Debug.Log(isFlashlightOn
            ? "<color=yellow>[ФОНАРИК] Свет включён</color>"
            : "<color=white>[ФОНАРИК] Свет выключен</color>");

        if (!isFlashlightOn)
            return;

        if (isMonsterPresent)
            ResolveThreat();
        else
            ThreatManager.Instance?.PlayerDefendedThreat(KitchenSide.Left);
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

        isFlashlightOn = false;
        ApplyFlashlightVisual(false);
        ViewRotationBlocker.SetBlock(false);
    }

    public void ResetThreat()
    {
        isMonsterPresent = false;
        isFlashlightOn = false;
        ApplyFlashlightVisual(false);
        ViewRotationBlocker.SetBlock(false);

        if (normalVentSprite != null)
            SetVentSprite(normalVentSprite);
    }
}
