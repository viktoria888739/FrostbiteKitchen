using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;
using FrostbiteKitchen.UI;

[Serializable]
public class CustomerProfile
{
    public string displayName;
    public Sprite portrait;
    public AudioClip greetingVoice;
}

public class IssueWindowCustomerPresenter : MonoBehaviour, IInteractable
{
    [SerializeField] private Image customerPortraitImage;
    [SerializeField] private GameObject dialogueWindowRoot;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private List<CustomerProfile> customerProfiles = new List<CustomerProfile>();
    [SerializeField] private string orderDialogueFormat = "Закажите:\n{0}";
    [SerializeField] private AudioSource voiceAudioSource;

    private CustomerProfile activeCustomer;

    private void Awake()
    {
        ResolveReferences();
        EnsureVoiceSource();
        EnsureDialogueText();
        EnsureClickableTargets();
    }

    public void Interact()
    {
        if (OrderManager.Instance == null || OrderManager.Instance.GetActiveRecipe() == null)
            return;

        OrderDelivery.TrySubmit();
    }

    private void OnEnable()
    {
        OrderManager.OnNewOrderStarted += HandleNewOrder;
        OrderManager.OnOrderSubmitted += HandleOrderFinished;
        OrderManager.OnOrderExpired += HandleOrderFinished;
        OrderManager.OnOrderFailed += HandleOrderFinished;
        SessionOrderTracker.OnSessionCompleted += HandleOrderFinishedVoid;
    }

    private void OnDisable()
    {
        OrderManager.OnNewOrderStarted -= HandleNewOrder;
        OrderManager.OnOrderSubmitted -= HandleOrderFinished;
        OrderManager.OnOrderExpired -= HandleOrderFinished;
        OrderManager.OnOrderFailed -= HandleOrderFinished;
        SessionOrderTracker.OnSessionCompleted -= HandleOrderFinishedVoid;
    }

    private void Start()
    {
        HideCustomer();
        EnsureDialogueText();

        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveRecipe() != null)
        {
            HandleNewOrder(OrderManager.Instance.GetActiveRecipe());
        }
    }

    private void ResolveReferences()
    {
        if (customerPortraitImage == null)
        {
            Transform customerTransform = transform.Find("Char_CustomerPlaceholder");
            if (customerTransform != null)
            {
                customerPortraitImage = customerTransform.GetComponent<Image>();
            }
        }

        if (dialogueWindowRoot == null)
        {
            Transform dialogueTransform = transform.Find("UI_DialogueWindowBox");
            if (dialogueTransform != null)
            {
                dialogueWindowRoot = dialogueTransform.gameObject;
            }
        }
    }

    private void EnsureVoiceSource()
    {
        if (voiceAudioSource != null)
        {
            return;
        }

        voiceAudioSource = GetComponent<AudioSource>();
        if (voiceAudioSource == null)
        {
            voiceAudioSource = gameObject.AddComponent<AudioSource>();
        }

        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.spatialBlend = 0f;
    }

    private void EnsureClickableTargets()
    {
        if (customerPortraitImage != null)
            customerPortraitImage.raycastTarget = true;

        if (dialogueWindowRoot == null)
            return;

        Image dialogueImage = dialogueWindowRoot.GetComponent<Image>();
        if (dialogueImage != null)
            dialogueImage.raycastTarget = true;
    }

    private void EnsureDialogueText()
    {
        if (dialogueText != null || dialogueWindowRoot == null)
        {
            return;
        }

        dialogueText = dialogueWindowRoot.GetComponentInChildren<TextMeshProUGUI>(true);
        if (dialogueText != null)
        {
            return;
        }

        GameObject textObject = new GameObject("DialogueText", typeof(RectTransform));
        textObject.transform.SetParent(dialogueWindowRoot.transform, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(12f, 12f);
        rect.offsetMax = new Vector2(-12f, -12f);

        dialogueText = textObject.AddComponent<TextMeshProUGUI>();
        dialogueText.font = TMP_Settings.defaultFontAsset;
        dialogueText.alignment = TextAlignmentOptions.Center;
        dialogueText.fontSize = 22f;
        dialogueText.color = Color.black;
        dialogueText.textWrappingMode = TextWrappingModes.Normal;
    }

    private void HandleNewOrder(RecipeData recipe)
    {
        if (recipe == null)
        {
            HideCustomer();
            return;
        }

        activeCustomer = PickRandomCustomer();
        if (activeCustomer == null)
            return;

        if (customerPortraitImage != null)
        {
            customerPortraitImage.sprite = activeCustomer.portrait;
            customerPortraitImage.color = Color.white;
            customerPortraitImage.gameObject.SetActive(true);
        }

        if (dialogueWindowRoot != null)
        {
            dialogueWindowRoot.SetActive(true);
        }

        if (dialogueText != null)
        {
            dialogueText.text = string.Format(orderDialogueFormat, recipe.recipeName);
            dialogueText.gameObject.SetActive(true);
        }

        PlayCustomerVoice(activeCustomer.greetingVoice);
    }

    private void HandleOrderFinished()
    {
        HideCustomer();
    }

    private void HandleOrderFinishedVoid()
    {
        HideCustomer();
    }

    private void HideCustomer()
    {
        if (customerPortraitImage != null)
        {
            customerPortraitImage.gameObject.SetActive(false);
        }

        if (dialogueWindowRoot != null)
        {
            dialogueWindowRoot.SetActive(false);
        }

        if (voiceAudioSource != null && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }

        activeCustomer = null;
    }

    private CustomerProfile PickRandomCustomer()
    {
        if (customerProfiles == null || customerProfiles.Count == 0)
        {
            return null;
        }

        List<CustomerProfile> validProfiles = customerProfiles.FindAll(
            profile => profile != null && profile.portrait != null);

        if (validProfiles.Count == 0)
        {
            return null;
        }

        return validProfiles[UnityEngine.Random.Range(0, validProfiles.Count)];
    }

    private void PlayCustomerVoice(AudioClip clip)
    {
        if (clip == null && GameAudioManager.Instance != null)
            clip = GameAudioManager.Instance.GetRandomCustomerVoice();

        if (clip == null || voiceAudioSource == null)
            return;

        voiceAudioSource.Stop();
        voiceAudioSource.clip = clip;
        voiceAudioSource.Play();
    }
}
