using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    private const string SavedVolumeKey = "SavedMasterVolume";
    private const string MasterVolumeParam = "MasterVolume";

    [SerializeField] private GameAudioLibrary library;
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup ambientMixerGroup;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource loopSource;
    private readonly HashSet<Button> wiredUiButtons = new HashSet<Button>();

    public AudioMixerGroup SfxMixerGroup => sfxMixerGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (library == null)
            library = Resources.Load<GameAudioLibrary>("GameAudioLibrary");

        ResolveMixerGroups();
        ApplySavedMasterVolume();

        sfxSource = CreateSource("SFX", sfxMixerGroup);
        musicSource = CreateSource("Music", musicMixerGroup, loop: true);
        ambientSource = CreateSource("Ambient", ambientMixerGroup, loop: true);
        loopSource = CreateSource("Loop", sfxMixerGroup, loop: true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        WireUiButtonSounds();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        WireUiButtonSounds();
    }

    private void ResolveMixerGroups()
    {
        if (mainMixer == null)
            return;

        AudioMixerGroup[] groups = mainMixer.FindMatchingGroups(string.Empty);
        foreach (AudioMixerGroup group in groups)
        {
            if (group == null)
                continue;

            switch (group.name)
            {
                case "SFX" when sfxMixerGroup == null:
                    sfxMixerGroup = group;
                    break;
                case "Music" when musicMixerGroup == null:
                    musicMixerGroup = group;
                    break;
                case "Ambient" when ambientMixerGroup == null:
                    ambientMixerGroup = group;
                    break;
            }
        }

        if (ambientMixerGroup == null)
            ambientMixerGroup = musicMixerGroup;
    }

    private void ApplySavedMasterVolume()
    {
        if (mainMixer == null)
            return;

        float savedVolume = PlayerPrefs.GetFloat(SavedVolumeKey, 0.5f);
        float dbValue = savedVolume > 0.0001f ? Mathf.Log10(savedVolume) * 20f : -80f;
        mainMixer.SetFloat(MasterVolumeParam, dbValue);
    }

    private void WireUiButtonSounds()
    {
        Button[] buttons = Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button == null || wiredUiButtons.Contains(button))
                continue;

            button.onClick.AddListener(PlayUiClick);
            wiredUiButtons.Add(button);
        }
    }

    private AudioSource CreateSource(string name, AudioMixerGroup group, bool loop = false)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(transform);

        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = loop;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = group;
        return source;
    }

    public void PlayTake() => PlaySfx(library?.take);
    public void PlayPlace() => PlaySfx(library?.place);
    public void PlayStoveDone() => PlaySfx(library?.stoveDone);
    public void PlayDishAssembled() => PlaySfx(library?.dishAssembled);
    public void PlayDishError() => PlaySfx(library?.dishError);
    public void PlayPlateServe() => PlaySfx(library?.plateServe);
    public void PlayTrashDrop() => PlaySfx(library?.trashDrop);

    public void PlayOrderNew() => PlaySfx(library?.orderNew);
    public void PlayOrderSuccess() => PlaySfx(library?.orderSuccess);
    public void PlayOrderWrong() => PlaySfx(library?.orderWrong);
    public void PlayOrderExpired() => PlaySfx(library?.orderExpired);

    public void PlayJumpscare() => PlaySfx(library?.jumpscare);
    public void PlayBlindsClose() => PlaySfx(library?.blindsClose);
    public void PlayBlindsOpen() => PlaySfx(library?.blindsOpen);
    public void PlayFlashlight() => PlaySfx(library?.flashlight);
    public void PlayExtinguisherSpray() => PlaySfx(library?.extinguisherSpray);
    public void PlayUiClick() => PlaySfx(library?.uiClick, 2f);

    public void PlaySessionWin() => PlaySfx(library?.sessionWin);
    public void PlaySessionGameOver() => PlaySfx(library?.sessionGameOver);

    public void PlayThreatSpawn(KitchenSide side)
    {
        if (library == null)
            return;

        AudioClip clip = side switch
        {
            KitchenSide.Front => library.threatSpawnFront,
            KitchenSide.Left => library.threatSpawnVent,
            KitchenSide.Back => library.threatSpawnWarehouse,
            KitchenSide.Right => library.threatSpawnKitchen,
            _ => library.threatSpawnKitchen
        };

        PlaySfx(clip);
        PlaySfx(library.tensionSting, 2f);
    }

    public void StartStoveSizzle()
    {
        if (library == null || library.stoveSizzleLoop == null)
            return;

        loopSource.clip = library.stoveSizzleLoop;
        loopSource.volume = 1f;
        loopSource.Play();
    }

    public void StopStoveSizzle()
    {
        if (loopSource.isPlaying)
            loopSource.Stop();
    }

    public void PlayMainMenuMusic()
    {
        StopAmbient();
        PlayMusic(library?.mainMenuMusic, 2f);
    }

    public void PlayKitchenAmbient()
    {
        StopMusic();
        PlayAmbient(library?.kitchenAmbientLoop);
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void StopAmbient()
    {
        if (ambientSource.isPlaying)
            ambientSource.Stop();
    }

    public void StopAllLoops()
    {
        StopStoveSizzle();
        StopMusic();
        StopAmbient();
    }

    public AudioClip GetRandomCustomerVoice()
    {
        if (library == null)
            return null;

        bool useMale = Random.value > 0.5f;
        AudioClip primary = useMale ? library.maleCustomerVoice : library.femaleCustomerVoice;
        AudioClip fallback = useMale ? library.femaleCustomerVoice : library.maleCustomerVoice;
        return primary != null ? primary : fallback;
    }

    public AudioClip GetCustomerVoice(CustomerProfile profile)
    {
        if (profile == null)
            return GetRandomCustomerVoice();

        if (profile.greetingVoice != null)
            return profile.greetingVoice;

        string customerKey = GetCustomerKey(profile);
        if (UsesFemaleCustomerVoice(customerKey))
            return GetFemaleCustomerVoice();

        if (UsesMaleCustomerVoice(customerKey))
            return GetMaleCustomerVoice();

        return GetRandomCustomerVoice();
    }

    public static string GetCustomerKey(CustomerProfile profile)
    {
        if (profile == null)
            return null;

        if (!string.IsNullOrWhiteSpace(profile.displayName))
            return profile.displayName.Trim();

        return profile.portrait != null ? profile.portrait.name : null;
    }

    public static bool UsesFemaleCustomerVoice(string customerKey)
    {
        return customerKey == "person 1" || customerKey == "person 4";
    }

    public static bool UsesMaleCustomerVoice(string customerKey)
    {
        return customerKey == "person 2" || customerKey == "person 3" || customerKey == "person 5";
    }

    public AudioClip GetMaleCustomerVoice() => library?.maleCustomerVoice;
    public AudioClip GetFemaleCustomerVoice() => library?.femaleCustomerVoice;

    private void PlayMusic(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null)
            return;

        musicSource.clip = clip;
        musicSource.volume = volumeMultiplier;
        musicSource.Play();
    }

    private void PlayAmbient(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null)
            return;

        ambientSource.clip = clip;
        ambientSource.volume = volumeMultiplier;
        ambientSource.Play();
    }

    private void PlaySfx(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }
}
