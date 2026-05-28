using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public Image background;
        public Image iconImage;
        public Text amountText;
    }

    [Header("Slot References")]
    [SerializeField] private SlotUI[] slots;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.84f, 0f, 1f);
    [SerializeField] private Color normalColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private bool isSubscribed = false;

    private void Awake()
    {
        if (slots == null || slots.Length == 0)
            Debug.LogError("InventoryUI: массив slots не назначен!");
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (isSubscribed) return;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSelectedSlotChanged += UpdateSelection;
            InventoryManager.Instance.OnSlotChanged += UpdateSlot;
            isSubscribed = true;
            Debug.Log("InventoryUI: подписка успешна");

            // Принудительно обновить UI
            UpdateSelection(InventoryManager.Instance.SelectedSlot);
            for (int i = 0; i < slots.Length; i++)
                UpdateSlot(i, InventoryManager.Instance.GetSlotItem(i));
        }
        else
        {
            // Повторить попытку через 0.2 секунды
            Invoke(nameof(TrySubscribe), 0.2f);
        }
    }

    private void OnDisable()
    {
        if (isSubscribed && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSelectedSlotChanged -= UpdateSelection;
            InventoryManager.Instance.OnSlotChanged -= UpdateSlot;
            isSubscribed = false;
        }
    }

    private void UpdateSelection(int slotIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].background != null)
                slots[i].background.color = (i == slotIndex) ? selectedColor : normalColor;
        }
    }

    private void UpdateSlot(int slotIndex, IngredientData item)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        var slot = slots[slotIndex];
        if (slot.iconImage != null)
        {
            if (item != null && item.icon != null)
            {
                slot.iconImage.sprite = item.icon;
                slot.iconImage.enabled = true;
            }
            else
            {
                slot.iconImage.sprite = null;
                slot.iconImage.enabled = false;
            }
        }
        if (slot.amountText != null)
            slot.amountText.text = "";
    }
}