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
    [SerializeField] private TMP_FontAsset dialogueFont;
    [SerializeField] private Color dialogueTextColor = Color.white;
    [SerializeField] private List<CustomerProfile> customerProfiles = new List<CustomerProfile>();
    [SerializeField] private string orderDialogueFormat = "Привет! Мне {0}.";
    [SerializeField] private string resourcesCustomersPath = "Customers";
    [SerializeField] private AudioSource voiceAudioSource;

    private CustomerProfile activeCustomer;

    private void Awake()
    {
        EnsureCustomerProfiles();
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

    private void EnsureCustomerProfiles()
    {
        if (customerProfiles != null && customerProfiles.Count > 0)
        {
            int validCount = customerProfiles.FindAll(p => p != null && p.portrait != null).Count;
            if (validCount > 0)
                return;
        }

        if (string.IsNullOrWhiteSpace(resourcesCustomersPath))
            return;

        Sprite[] sprites = Resources.LoadAll<Sprite>(resourcesCustomersPath);
        if (sprites == null || sprites.Length == 0)
            return;

        customerProfiles = new List<CustomerProfile>();
        foreach (Sprite sprite in sprites)
        {
            if (sprite == null)
                continue;

            customerProfiles.Add(new CustomerProfile
            {
                displayName = sprite.name,
                portrait = sprite
            });
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
            ApplyDialogueStyle();
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
        dialogueText.alignment = TextAlignmentOptions.Center;
        dialogueText.fontSize = 22f;
        dialogueText.textWrappingMode = TextWrappingModes.Normal;
        ApplyDialogueStyle();
    }

    private void ApplyDialogueStyle()
    {
        if (dialogueText == null)
            return;

        dialogueText.color = dialogueTextColor;

        if (dialogueFont != null)
        {
            dialogueText.font = dialogueFont;
            return;
        }

        TMP_FontAsset projectFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/stolzl_medium SDF");
        if (projectFont != null)
            dialogueText.font = projectFont;
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
            ApplyDialogueStyle();
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
