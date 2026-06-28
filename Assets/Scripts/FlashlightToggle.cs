using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

public class FlashlightToggle : MonoBehaviour, IInteractable, IPointerClickHandler
{
    [SerializeField] private LeftThreatHandler leftThreatHandler;

    private void Awake()
    {
        if (leftThreatHandler == null)
            leftThreatHandler = Object.FindFirstObjectByType<LeftThreatHandler>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleFlashlight();
    }

    public void Interact()
    {
        ToggleFlashlight();
    }

    public void ToggleFlashlight()
    {
        if (leftThreatHandler == null)
            leftThreatHandler = Object.FindFirstObjectByType<LeftThreatHandler>();

        if (leftThreatHandler != null)
            leftThreatHandler.ToggleFlashlight();
        else
            Debug.LogWarning("[ФОНАРИК] LeftThreatHandler не найден на сцене.");
    }
}
