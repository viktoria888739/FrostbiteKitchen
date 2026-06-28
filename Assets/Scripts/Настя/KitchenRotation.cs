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
    [SerializeField] private LeftThreatHandler leftThreatHandler;

    private int currentViewIndex = 0;
    private GameObject[] views;

    void Start()
    {
        views = new GameObject[] { viewFront, viewRight, viewBack, viewLeft };

        if (warningManager == null)
            warningManager = Object.FindFirstObjectByType<VisualWarningManager>();

        if (leftThreatHandler == null)
            leftThreatHandler = Object.FindFirstObjectByType<LeftThreatHandler>();

        UpdateVisuals();
    }

    void Update()
    {
        if (ViewRotationBlocker.IsRotationBlocked)
            return;

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
        if (CameraWobbleEffect.Instance != null)
        {
            CameraWobbleEffect.Instance.PlayWobble(1);
        }

        currentViewIndex = (currentViewIndex + 1) % views.Length;
        UpdateVisuals();
    }

    public void RotateLeft()
    {
        if (CameraWobbleEffect.Instance != null)
        {
            CameraWobbleEffect.Instance.PlayWobble(-1);
        }

        currentViewIndex--;
        if (currentViewIndex < 0)
        {
            currentViewIndex = views.Length - 1;
        }
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
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

        KitchenSide currentSide = GetCurrentSideEnum(currentViewIndex);

        if (warningManager != null)
            warningManager.UpdatePlayerView(currentSide);

        if (currentSide != KitchenSide.Left)
            leftThreatHandler?.OnPlayerTurnAway();
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