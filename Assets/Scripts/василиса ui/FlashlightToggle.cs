using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class FlashlightToggle : MonoBehaviour, IInteractable
{
    [SerializeField] private LeftThreatHandler leftThreatHandler;

    private int lastInteractFrame = -1;

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
