using UnityEngine;
using UnityEngine.InputSystem; // Добавили поддержку новой системы ввода

public class KitchenRotation : MonoBehaviour
{
    [Header("Панели сторон кухни")]
    public GameObject viewFront;
    public GameObject viewRight;
    public GameObject viewLeft;
    public GameObject viewBack;

    [Header("Внешние ссылки")]
    [SerializeField] private VisualWarningManager warningManager;

    private int currentViewIndex = 0;
    private GameObject[] views;

    void Start()
    {
        // Порядок в массиве: 0 = Front, 1 = Right, 2 = Back, 3 = Left
        views = new GameObject[] { viewFront, viewRight, viewBack, viewLeft };

        // Автоматически ищем VisualWarningManager на сцене, если забыли привязать в инспекторе
        if (warningManager == null)
        {
            warningManager = Object.FindFirstObjectByType<VisualWarningManager>();
        }

        UpdateVisuals();
    }

    void Update()
    {
        // Проверяем нажатие клавиш по правилам New Input System в Unity 6
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                RotateLeft();
            }
            if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                RotateRight();
            }
        }
    }

    public void RotateRight()
    {
        currentViewIndex = (currentViewIndex + 1) % views.Length;
        UpdateVisuals();
    }

    public void RotateLeft()
    {
        currentViewIndex--;
        if (currentViewIndex < 0)
        {
            currentViewIndex = views.Length - 1;
        }
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] != null)
            {
                views[i].SetActive(i == currentViewIndex);
            }
        }

        // НОВАЯ ЛОГИКА: Сообщаем менеджеру предупреждений, куда теперь смотрит игрок
        if (warningManager != null)
        {
            KitchenSide currentSide = GetCurrentSideEnum(currentViewIndex);
            warningManager.UpdatePlayerView(currentSide);
        }
    }

    /// <summary>
    /// Конвертирует индекс массива в соответствующий KitchenSide Enum
    /// </summary>
    private KitchenSide GetCurrentSideEnum(int index)
    {
        switch (index)
        {
            case 0: return KitchenSide.Front;
            case 1: return KitchenSide.Right;
            case 2: return KitchenSide.Back;
            case 3: return KitchenSide.Left;
            default: return KitchenSide.Front;
        }
    }
}