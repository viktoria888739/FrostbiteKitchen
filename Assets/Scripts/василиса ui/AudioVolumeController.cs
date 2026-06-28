using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        LoadVolume();
    }

    private void OnEnable()
    {
        LoadVolume();
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SavedMasterVolume", 0.5f);

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        
        ApplyMixerVolume(savedVolume);
    }

    public void SetVolume(float sliderValue)
    {
        ApplyMixerVolume(sliderValue);
        
        PlayerPrefs.SetFloat("SavedMasterVolume", sliderValue);
        PlayerPrefs.Save();
    }

    private void ApplyMixerVolume(float value)
    {
        if (mainMixer == null) return;

        float dbValue = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        mainMixer.SetFloat("MasterVolume", dbValue);
    }
}