using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-50)]
public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [SerializeField] private GameAudioLibrary library;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup ambientMixerGroup;

    private AudioSource sfxSource;
    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource loopSource;

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

        sfxSource = CreateSource("SFX", sfxMixerGroup);
        musicSource = CreateSource("Music", musicMixerGroup, loop: true);
        ambientSource = CreateSource("Ambient", ambientMixerGroup, loop: true);
        loopSource = CreateSource("Loop", sfxMixerGroup, loop: true);
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
