using UnityEngine;
using FrostbiteKitchen.Threats;

public class ThreatCoordinator : MonoBehaviour
{
    [SerializeField] private LeftThreatHandler leftThreatHandler;
    [SerializeField] private BlindsToggle blindsToggle;

    private void Awake()
    {
        if (leftThreatHandler == null)
            leftThreatHandler = LeftThreatHandler.Instance;

        if (leftThreatHandler == null)
            leftThreatHandler = Object.FindFirstObjectByType<LeftThreatHandler>(FindObjectsInactive.Include);

        if (blindsToggle == null)
            blindsToggle = Object.FindFirstObjectByType<BlindsToggle>();
    }

    private void OnEnable()
    {
        ThreatManager.OnThreatStarted += HandleThreatStarted;
        ThreatManager.OnThreatCleared += HandleThreatCleared;
        ThreatManager.OnThreatFailed += HandleThreatFailed;
    }

    private void OnDisable()
    {
        ThreatManager.OnThreatStarted -= HandleThreatStarted;
        ThreatManager.OnThreatCleared -= HandleThreatCleared;
        ThreatManager.OnThreatFailed -= HandleThreatFailed;
    }

    private void HandleThreatStarted(KitchenSide side)
    {
        if (leftThreatHandler == null)
            leftThreatHandler = LeftThreatHandler.Instance;

        if (side == KitchenSide.Left)
            leftThreatHandler?.OnMonsterSpawn();
    }

    private void HandleThreatCleared()
    {
        leftThreatHandler?.ResetThreat();
    }

    private void HandleThreatFailed()
    {
        leftThreatHandler?.ResetThreat();
        blindsToggle?.ForceOpen();
        ViewRotationBlocker.Reset();
    }
}
