using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] private Image[] slotBackgrounds;
    [SerializeField] private Image[] slotIcons;
    [SerializeField] private TMP_Text[] slotAmountTexts;
    [SerializeField] private Color selectedColor = new Color(0.67f, 0.67f, 0.67f, 1f);
    [SerializeField] private Color defaultColor = Color.white;

    private int currentIndex;

    private void Start()
    {
        if (PlayerInventory.Instance != null)
        {
            currentIndex = PlayerInventory.Instance.SelectedSlotIndex;
            PlayerInventory.Instance.OnInventoryChanged += RefreshHotbarUI;
            PlayerInventory.Instance.OnSelectedSlotChanged += HandleSelectedSlotChanged;
        }

        RefreshHotbarUI();
    }

    private void OnDestroy()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged -= RefreshHotbarUI;
            PlayerInventory.Instance.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
        }
    }

    private void Update()
    {
        HandleNumberInput();
        HandleScrollInput();
    }

    private void HandleSelectedSlotChanged(int index)
    {
        currentIndex = index;
        RefreshSelectionVisual();
    }

    private void HandleNumberInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[Key.Digit1].wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame) SelectSlot(4 - 1);
    }

    private void HandleScrollInput()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f)
        {
            int newIndex = currentIndex - 1;
            if (newIndex < 0)
                newIndex = PlayerInventory.SlotCount - 1;

            SelectSlot(newIndex);
        }
        else if (scroll < 0f)
        {
            int newIndex = currentIndex + 1;
            if (newIndex >= PlayerInventory.SlotCount)
                newIndex = 0;

            SelectSlot(newIndex);
        }
    }

    private void SelectSlot(int index)
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.SelectSlot(index);
            return;
        }

        currentIndex = index;
        RefreshHotbarUI();
    }

    private void RefreshHotbarUI()
    {
        if (PlayerInventory.Instance == null)
        {
            RefreshSelectionVisual();
            return;
        }

        currentIndex = PlayerInventory.Instance.SelectedSlotIndex;

        for (int i = 0; i < slotIcons.Length; i++)
        {
            InventorySlot slot = PlayerInventory.Instance.GetSlot(i);
            Sprite icon = slot.GetIcon();

            if (slotIcons[i] != null)
            {
                slotIcons[i].sprite = icon;
                slotIcons[i].enabled = icon != null;
                slotIcons[i].color = icon != null ? Color.white : new Color(1f, 1f, 1f, 0f);
            }

            if (slotAmountTexts != null && i < slotAmountTexts.Length && slotAmountTexts[i] != null)
            {
                bool showAmount = slot.IsIngredient && slot.amount > 1;
                slotAmountTexts[i].gameObject.SetActive(showAmount);
                slotAmountTexts[i].text = showAmount ? slot.amount.ToString() : string.Empty;
            }
        }

        RefreshSelectionVisual();
    }

    private void RefreshSelectionVisual()
    {
        if (slotBackgrounds == null)
            return;

        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (slotBackgrounds[i] != null)
                slotBackgrounds[i].color = i == currentIndex ? selectedColor : defaultColor;
        }
    }

    public Sprite GetCurrentItemIcon()
    {
        if (PlayerInventory.Instance == null)
            return null;

        return PlayerInventory.Instance.GetSlot(currentIndex).GetIcon();
    }
}
