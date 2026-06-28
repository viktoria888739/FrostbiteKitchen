using UnityEngine;

[CreateAssetMenu(fileName = "GameAudioLibrary", menuName = "Frostbite Kitchen/Audio Library")]
public class GameAudioLibrary : ScriptableObject
{
    [Header("Ингредиенты и готовка")]
    public AudioClip take;
    public AudioClip place;
    public AudioClip stoveSizzleLoop;
    public AudioClip stoveDone;
    public AudioClip dishAssembled;
    public AudioClip dishError;
    public AudioClip plateServe;
    public AudioClip trashDrop;

    [Header("Заказы")]
    public AudioClip orderNew;
    public AudioClip orderSuccess;
    public AudioClip orderWrong;
    public AudioClip orderExpired;

    [Header("Угрозы")]
    public AudioClip threatSpawnKitchen;
    public AudioClip threatSpawnFront;
    public AudioClip threatSpawnVent;
    public AudioClip threatSpawnWarehouse;
    public AudioClip jumpscare;

    [Header("Защита")]
    public AudioClip blindsClose;
    public AudioClip blindsOpen;
    public AudioClip flashlight;
    public AudioClip extinguisherSpray;

    [Header("UI и сессия")]
    public AudioClip uiClick;
    public AudioClip sessionWin;
    public AudioClip sessionGameOver;

    [Header("Музыка и эмбиент")]
    public AudioClip mainMenuMusic;
    public AudioClip kitchenAmbientLoop;
    public AudioClip tensionSting;

    [Header("Голоса клиентов (не в таблице)")]
    public AudioClip maleCustomerVoice;
    public AudioClip femaleCustomerVoice;
}
