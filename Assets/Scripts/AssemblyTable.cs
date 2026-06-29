using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.Data;

namespace FrostbiteKitchen.KitchenStation
{
    public class AssemblyTable : MonoBehaviour, IInteractable, IPointerClickHandler
    {
        public static AssemblyTable Instance { get; private set; }

        [SerializeField] private int maxIngredients = 6;
        [SerializeField] private AssemblyTableVisual tableVisual;

        private int lastInteractFrame = -1;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (tableVisual == null)
                tableVisual = GetComponentInChildren<AssemblyTableVisual>(true);

            EnsureClickZone();
            ImageRaycastHelper.EnsureRaycastTarget(gameObject);
        }

        private void Start()
        {
            transform.SetAsLastSibling();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
        }

        public void Interact()
        {
            if (Time.frameCount == lastInteractFrame)
                return;

            lastInteractFrame = Time.frameCount;

            PlayerInventory inventory = PlayerInventory.Instance;
            DishAssembler assembler = DishAssembler.Instance;

            if (inventory == null || assembler == null)
                return;

            if (inventory.CurrentHeldDish != null)
                return;

            if (inventory.CurrentHeldItem == null && inventory.CurrentHeldDish == null &&
                (assembler.HasCompleteDish() || assembler.IsCurrentPlateSpoiled()))
            {
                TryPickupDish(inventory, assembler);
                return;
            }

            IngredientData heldItem = ResolveHeldIngredient(inventory);
            if (heldItem == null)
                return;

            if (assembler.GetCurrentIngredientCount() >= maxIngredients)
                return;

            if (heldItem.RequiresCooking)
                return;

            if (!assembler.TryAddIngredient(heldItem))
                return;

            inventory.TryUseOneItem();
            GameAudioManager.Instance?.PlayDishAssembled();
        }

        public void ResetTable()
        {
            tableVisual?.RefreshFromPlate();
        }

        public static bool CanAcceptHeldIngredient()
        {
            PlayerInventory inventory = PlayerInventory.Instance;
            if (inventory == null || inventory.CurrentHeldDish != null)
                return false;

            IngredientData item = inventory.CurrentHeldItem;
            if (item == null)
            {
                for (int i = 0; i < PlayerInventory.SlotCount; i++)
                {
                    InventorySlot slot = inventory.GetSlot(i);
                    if (!slot.IsIngredient)
                        continue;

                    item = slot.ingredient;
                    break;
                }
            }

            return item != null && !item.RequiresCooking;
        }

        public Sprite GetSpoiledDishSprite()
        {
            return tableVisual != null ? tableVisual.SpoiledDishSprite : null;
        }

        private void TryPickupDish(PlayerInventory inventory, DishAssembler assembler)
        {
            if (!assembler.TryCreateDishFromPlate(out DishData dish))
                return;

            if (!inventory.TryAddDish(dish))
                return;

            assembler.ClearPlate();
            ResetTable();
            GameAudioManager.Instance?.PlayTake();
        }

        private void EnsureClickZone()
        {
            Transform existing = transform.Find("ClickZone");
            if (existing != null)
                return;

            GameObject clickZone = new GameObject("ClickZone", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            clickZone.transform.SetParent(transform, false);

            RectTransform rect = clickZone.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.SetAsLastSibling();

            UnityEngine.UI.Image image = clickZone.GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(1f, 1f, 1f, 0.01f);
            image.raycastTarget = true;
        }

        private static IngredientData ResolveHeldIngredient(PlayerInventory inventory)
        {
            if (inventory.CurrentHeldItem != null)
                return inventory.CurrentHeldItem;

            for (int i = 0; i < PlayerInventory.SlotCount; i++)
            {
                InventorySlot slot = inventory.GetSlot(i);
                if (!slot.IsIngredient)
                    continue;

                inventory.SelectSlot(i);
                return slot.ingredient;
            }

            return null;
        }
    }
}
