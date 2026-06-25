using UnityEngine;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTable : MonoBehaviour, IInteractable
    {
        [Header("Сборочный стол")]
        [Tooltip("Максимальное количество ингредиентов на тарелке")]
        [SerializeField] private int maxIngredients = 6;

        public void Interact()
        {
            var inventory = PlayerInventory.Instance;
            var assembler = DishAssembler.Instance;

            if (inventory == null || assembler == null)
            {
                Debug.LogWarning("[СБОРКА] Не найдены Inventory или DishAssembler!");
                return;
            }

            if (inventory.CurrentHeldItem == null)
            {
                Debug.Log("<color=#33FF57>[СБОРКА]</color> Руки пустые.");
                return;
            }

            if (assembler.GetCurrentIngredientCount() >= maxIngredients)
            {
                Debug.Log("<color=yellow>[СБОРКА]</color> Тарелка уже полная!");
                return;
            }

            IngredientData heldItem = inventory.CurrentHeldItem;
            assembler.AddIngredient(heldItem);
            inventory.TryUseOneItem();
            Debug.Log($"<color=#33FF57>[СБОРКА]</color> Добавлен ингредиент: {heldItem.displayName}");
        }
    }
}