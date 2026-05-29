using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action<int> OnSelectedSlotChanged;
    public event Action<int, IngredientData> OnSlotChanged;

    [Header("Settings")]
    [SerializeField] private int slotCount = 4;
    [SerializeField] private IngredientData[] slots;

    private int selectedSlotIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (slots == null || slots.Length != slotCount)
            slots = new IngredientData[slotCount];
    }

    private void Start()
    {
        for (int i = 0; i < slotCount; i++)
            OnSlotChanged?.Invoke(i, slots[i]);

        selectedSlotIndex = 0;
        OnSelectedSlotChanged?.Invoke(selectedSlotIndex);
    }

    private void Update()
    {
        // Колесико мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int delta = scroll > 0 ? -1 : 1;
            int newIndex = selectedSlotIndex + delta;
            if (newIndex < 0) newIndex = slotCount - 1;
            if (newIndex >= slotCount) newIndex = 0;
            SelectSlot(newIndex);
        }

        // Цифровые клавиши 1-4
        for (int i = 0; i < slotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotCount) return;
        if (selectedSlotIndex == index) return;

        selectedSlotIndex = index;
        OnSelectedSlotChanged?.Invoke(selectedSlotIndex);

        if (HandItemManager.Instance != null)
            HandItemManager.Instance.SetHandItem(slots[selectedSlotIndex]);
    }

    public IngredientData GetSlotItem(int index)
    {
        if (index < 0 || index >= slotCount) return null;
        return slots[index];
    }

    public void SetSlotItem(int index, IngredientData item)
    {
        if (index < 0 || index >= slotCount) return;
        slots[index] = item;
        OnSlotChanged?.Invoke(index, item);

        if (index == selectedSlotIndex && HandItemManager.Instance != null)
            HandItemManager.Instance.SetHandItem(item);
    }

    public int SelectedSlot => selectedSlotIndex;
}