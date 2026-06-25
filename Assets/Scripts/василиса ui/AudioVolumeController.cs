using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeController : MonoBehaviour
{
    [Header("Ссылка на Аудио Микшер")]
    [SerializeField] private AudioMixer mainMixer;

    public void SetVolume(float sliderValue)
    {
        if (mainMixer == null)
        {
            Debug.LogWarning("[AUDIO] AudioMixer не назначен в инспекторе!");
            return;
        }

        float dbValue = Mathf.Log10(sliderValue) * 20f;

        mainMixer.SetFloat("MasterVolume", dbValue);
    }
}