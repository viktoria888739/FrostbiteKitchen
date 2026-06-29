using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class TrashBin : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null || inventory.IsEmpty)
            return;

        inventory.ClearSelectedSlot();
        GameAudioManager.Instance?.PlayTrashDrop();
    }
}
