using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    [SerializeField] private Image[] slotBackgrounds;
    [SerializeField] private Image[] slotIcons;
    [SerializeField] private Sprite[] items;
    [SerializeField] private Color selectedColor = Color.gray7;
    [SerializeField] private Color defaultColor = Color.white;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateHotbarUI();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        HandleNumberInput();
        HandleScrollInput();
    }

    private void HandleNumberInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[Key.Digit1].wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current[Key.Digit2].wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current[Key.Digit3].wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current[Key.Digit4].wasPressedThisFrame) SelectSlot(3);
    }

    private void HandleScrollInput()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f)
        {
            int newIndex = currentIndex - 1;
            if (newIndex < 0) newIndex = 3;
            SelectSlot(newIndex);
        }
        else if (scroll < 0f)
        {
            int newIndex = currentIndex + 1;
            if (newIndex > 3) newIndex = 0;
            SelectSlot(newIndex);
        }
    }

    private void SelectSlot(int index)
    {
        currentIndex = index;
        UpdateHotbarUI();
    }

    private void UpdateHotbarUI()
    {
        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (i >= slotIcons.Length) continue;

            slotBackgrounds[i].color = i == currentIndex ? selectedColor : defaultColor;

            if (i < items.Length && items[i] != null)
            {
                slotIcons[i].sprite = items[i];
                slotIcons[i].enabled = true;
            }
            else
            {
                slotIcons[i].sprite = null;
                slotIcons[i].enabled = false;
            }
        }
    }

    public Sprite GetCurrentItem()
    {
        if (currentIndex < items.Length)
        {
            return items[currentIndex];
        }
        return null;
    }
}