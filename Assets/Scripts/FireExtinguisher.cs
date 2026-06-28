using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

public class FireExtinguisher : MonoBehaviour, IInteractable
{
    [Header("Настройки")]
    [SerializeField] private ParticleSystem sprayEffect;

    public void Interact()
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#FF3333>[ОГНЕТУШИТЕЛЬ] Применён! Угроза нейтрализована.</color>");

        if (sprayEffect != null)
            sprayEffect.Play();

        GameAudioManager.Instance?.PlayExtinguisherSpray();

        if (ThreatManager.Instance != null)
            ThreatManager.Instance.PlayerDefendedThreat(KitchenSide.Back);

    }
}