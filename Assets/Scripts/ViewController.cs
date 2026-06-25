using UnityEngine;
using UnityEngine.InputSystem;

public class ViewController : MonoBehaviour
{
    public static ViewController Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private float rotationCooldown = 0.25f;

    private float lastRotationTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (ViewRotationBlocker.IsRotationBlocked)
            return;

        HandleRotationInput();
    }

    private void HandleRotationInput()
    {
        if (Time.time - lastRotationTime < rotationCooldown)
            return;

        if (Keyboard.current != null && 
            (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame))
        {
            RotateView(-1);
        }

        if (Keyboard.current != null && 
            (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame))
        {
            RotateView(1);
        }
    }

    private void RotateView(int direction)
    {
        lastRotationTime = Time.time;

        if (CameraWobbleEffect.Instance != null)
        {
            CameraWobbleEffect.Instance.PlayWobble(direction);
        }

        Debug.Log($"[ViewController] Поворот вида: {(direction > 0 ? "Вправо" : "Влево")}");
    }
}