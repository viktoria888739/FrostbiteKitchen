using UnityEngine;
using UnityEngine.UI;
using FrostbiteKitchen.Gameplay;

public class FlashlightToggle : MonoBehaviour, IInteractable
{
    [Header("Объект света")]
    [SerializeField] private GameObject spotlightObject;

    [Header("Иконка внутри кнопки")]
    [Tooltip("Ссылка на дочерний объект Image")]
    [SerializeField] private Image innerIconImage; 
    [SerializeField] private Sprite iconOff;
    [SerializeField] private Sprite iconOn;

    private bool isOn = false;

    private void Start()
    {
        if (innerIconImage == null)
        {
            Transform childTransform = transform.Find("Image");
            if (childTransform != null)
            {
                innerIconImage = childTransform.GetComponent<Image>();
            }
        }
        
        if (innerIconImage != null && iconOff != null)
        {
            innerIconImage.sprite = iconOff;
        }
        
        if (spotlightObject != null && spotlightObject != this.gameObject)
        {
            spotlightObject.SetActive(false);
        }
    }

    public void Interact()
    {
        ToggleFlashlight();
    }

    public void ToggleFlashlight()
    {
        if (spotlightObject != null)
        {
            isOn = !isOn;
            spotlightObject.SetActive(isOn);
            ViewRotationBlocker.SetBlock(isOn);
            if (innerIconImage != null)
            {
                innerIconImage.sprite = isOn ? iconOn : iconOff;
            }
            if (isOn)
            {
                Debug.Log("<color=yellow>[ФОНАРИК] Свет включён — вращение заблокировано</color>");
                if (ThreatManager.Instance != null)
                {
                    ThreatManager.Instance.PlayerDefendedThreat(null);
                }
            }
            else
            {
                Debug.Log("<color=white>[ФОНАРИК] Свет выключен — вращение разрешено</color>");
            }
        }
    }
}