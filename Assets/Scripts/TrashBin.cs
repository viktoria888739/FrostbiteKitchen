using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class TrashBin : MonoBehaviour, IInteractable
{
    [Header("Мусорка")]
    [Tooltip("Звук выброса (опционально)")]
    [SerializeField] private AudioClip trashSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && trashSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Interact()
    {
        var inventory = PlayerInventory.Instance;

        if (inventory.CurrentHeldItem != null)
        {
            string itemName = inventory.CurrentHeldItem.displayName;

            inventory.ClearInventory();

            Debug.Log($"<color=gray>[МУСОРКА]</color> Выброшено: {itemName}");

            if (audioSource != null && trashSound != null)
            {
                audioSource.PlayOneShot(trashSound);
            }
        }
        else
        {
            Debug.Log("<color=gray>[МУСОРКА]</color> В руках ничего нет.");
        }
    }
}