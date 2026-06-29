using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class FireExtinguisher : MonoBehaviour, IInteractable
{
    [Header("Настройки")]
    [SerializeField] private ParticleSystem sprayEffect;

    private int lastInteractFrame = -1;

    public void Interact()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (Time.frameCount == lastInteractFrame)
            return;

        lastInteractFrame = Time.frameCount;

        if (sprayEffect != null)
            sprayEffect.Play();

        GameAudioManager.Instance?.PlayExtinguisherSpray();

        if (ThreatManager.Instance != null && ThreatManager.Instance.IsActiveThreatOn(KitchenSide.Right))
        {
            Debug.Log("<color=#FF3333>[ОГНЕТУШИТЕЛЬ] Угроза на кухне нейтрализована.</color>");
            ThreatManager.Instance.PlayerDefendedThreat(KitchenSide.Right);
        }
    }
}
