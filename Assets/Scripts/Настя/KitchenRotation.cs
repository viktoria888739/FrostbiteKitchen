using UnityEngine;
using UnityEngine.InputSystem;

public class KitchenRotation : MonoBehaviour
{
    [Header("Виды сторон кухни")]
    public GameObject viewFront;
    public GameObject viewRight;
    public GameObject viewLeft;
    public GameObject viewBack;

    [Header("Менеджер предупреждений")]
    [SerializeField] private VisualWarningManager warningManager;

    private int currentViewIndex = 0;
    private GameObject[] views;

    void Start()
    {
        views = new GameObject[] { viewFront, viewRight, viewBack, viewLeft };

        if (warningManager == null)
        {
            warningManager = Object.FindFirstObjectByType<VisualWarningManager>();
        }

        UpdateVisuals();
    }

    void Update()
    {
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
        // Вызываем эффект затемнения перед обновлением геометрии
        if (CameraBlinkEffect.Instance != null)
        {
            CameraBlinkEffect.Instance.PlayBlink();
        }

        for (int i = 0; i < views.Length; i++)
        {
            if (views[i] != null)
            {
                views[i].SetActive(i == currentViewIndex);
            }
        }

        if (warningManager != null)
        {
            KitchenSide currentSide = GetCurrentSideEnum(currentViewIndex);
            warningManager.UpdatePlayerView(currentSide);
        }
    }

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