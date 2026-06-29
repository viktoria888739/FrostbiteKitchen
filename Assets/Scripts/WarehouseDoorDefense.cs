using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class WarehouseDoorDefense : MonoBehaviour, IInteractable
{
    private int lastInteractFrame = -1;

    public void Interact()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (Time.frameCount == lastInteractFrame)
            return;

        lastInteractFrame = Time.frameCount;

        if (ThreatManager.Instance == null || !ThreatManager.Instance.IsActiveThreatOn(KitchenSide.Back))
            return;

        Debug.Log("<color=green>[ДВЕРЬ] Склад заперт — монстр отражён.</color>");
        ThreatManager.Instance.PlayerDefendedThreat(KitchenSide.Back);
    }
}
