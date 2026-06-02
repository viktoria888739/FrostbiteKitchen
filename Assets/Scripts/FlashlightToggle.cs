using UnityEngine;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;
public class FlashlightToggle : MonoBehaviour, IInteractable
{
    [Header("Объект света")]
    [SerializeField] private GameObject spotlightObject;

    private bool isOn = false;

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

            if (isOn)
            {
                Debug.Log("<color=yellow>[ФОНАРИК]</color> Свет включен в вентиляции");
            }
            else
            {
                Debug.Log("<color=white>[ФОНАРИК]</color> Свет выключен");
            }
        }
    }
}