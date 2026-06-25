using UnityEngine;
using FrostbiteKitchen.Gameplay;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Объект света")]
    [SerializeField] private GameObject spotlightObject;

    private bool isOn = false;

    // Этот метод мы будем вызывать при нажатии на UI Button
    public void ToggleFlashlight()
    {
        if (spotlightObject != null)
        {
            isOn = !isOn;
            spotlightObject.SetActive(isOn);

            if (isOn)
            {
                Debug.Log("<color=yellow>[ФОНАРИК]</color> Свет включён в вентиляции");

                // Сохраняем логику Вики: если фонарик включили, 
                // отправляем сигнал в ThreatManager, что мы защитились от монстра
                if (ThreatManager.Instance != null)
                    ThreatManager.Instance.PlayerDefendedThreat(null);
            }
            else
            {
                Debug.Log("<color=white>[ФОНАРИК]</color> Свет выключен");
            }
        }
    }
}