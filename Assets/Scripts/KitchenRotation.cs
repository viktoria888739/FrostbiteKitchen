using UnityEngine;
using UnityEngine.InputSystem; // Добавили поддержку новой системы ввода

public class KitchenRotation : MonoBehaviour
{
    [Header("Панели сторон кухни")]
    public GameObject viewFront;
    public GameObject viewRight;
    public GameObject viewLeft;
    public GameObject viewBack;

    private int currentViewIndex = 0;
    private GameObject[] views;

    void Start()
    {
        views = new GameObject[] { viewFront, viewRight, viewBack, viewLeft };
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
    }
}