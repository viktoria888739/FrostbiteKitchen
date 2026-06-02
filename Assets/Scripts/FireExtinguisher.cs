using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
public class FireExtinguisher : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        if (!gameObject.activeInHierarchy) return;

        Debug.Log("<color=#FF3333>[ОГНЕТУШИТЕЛЬ]</color> Взаимодействие с огнетушителем. Взят в руки.");
        // Здесь в будущем будет вызов метода из инвентаря/рук игрока: Inventory.Instance.SetItem(this);
    }
}