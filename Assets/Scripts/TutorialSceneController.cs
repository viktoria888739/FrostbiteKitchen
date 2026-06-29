using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using FrostbiteKitchen.Threats;
using FrostbiteKitchen.UI;

public class TutorialSceneController : MonoBehaviour
{
    public const int GameplaySceneIndex = 2;

    private static readonly string[] DialogueLines =
    {
        "Эй. Ты новенький? Не пугайся, я объясню быстро.",
        "Я буду заказывать еду. Ты готовишь — я жду здесь у окна.",
        "Кухня крутится. Жми A и D или стрелки, чтобы смотреть по сторонам.",
        "Сзади склад. Бери коробки с продуктами.",
        "На столах раскладывай всё, что понадобится для готовки.",
        "На плите жарь. Передержишь, и всё сгорит.",
        "На сборочном столе собери блюдо по рецепту.",
        "Готовое блюдо подай мне в это окно.",
        "У каждого заказа есть время. Опоздаешь — заказ провален.",
        "И смотри по сторонам. В этой кухне иногда бывает… лишнее.",
        "Ладно. Хватит теории. Пора работать."
    };

    [SerializeField] private Transform viewFrontRoot;
    [SerializeField] private Image customerPortraitImage;
    [SerializeField] private GameObject dialogueWindowRoot;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Sprite tutorialCustomerPortrait;
    [SerializeField] private TMP_FontAsset dialogueFont;
    [SerializeField] private int gameplaySceneIndex = GameplaySceneIndex;

    private int currentLineIndex;

    private void Awake()
    {
        ResolveReferences();
        DisableGameplayBehaviours();
        PrepareDialogueUi();
        ShowLine(0);
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.rightButton.wasPressedThisFrame)
            return;

        AdvanceDialogue();
    }

    private void ResolveReferences()
    {
        if (viewFrontRoot == null)
        {
            GameObject viewFrontObject = GameObject.Find("View_Front");
            if (viewFrontObject != null)
                viewFrontRoot = viewFrontObject.transform;
        }

        if (viewFrontRoot == null)
            return;

        if (customerPortraitImage == null)
        {
            Transform portraitTransform = viewFrontRoot.Find("Char_CustomerPlaceholder");
            if (portraitTransform != null)
                customerPortraitImage = portraitTransform.GetComponent<Image>();
        }

        if (dialogueWindowRoot == null)
        {
            Transform dialogueTransform = viewFrontRoot.Find("UI_DialogueWindowBox");
            if (dialogueTransform != null)
                dialogueWindowRoot = dialogueTransform.gameObject;
        }

        if (dialogueText == null && dialogueWindowRoot != null)
            dialogueText = dialogueWindowRoot.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void PrepareDialogueUi()
    {
        if (tutorialCustomerPortrait == null)
            tutorialCustomerPortrait = Resources.Load<Sprite>("Customers/person 1");

        if (dialogueFont == null)
            dialogueFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/stolzl_book SDF");

        if (customerPortraitImage != null)
        {
            if (tutorialCustomerPortrait != null)
            {
                customerPortraitImage.sprite = tutorialCustomerPortrait;
                customerPortraitImage.color = Color.white;
            }

            customerPortraitImage.raycastTarget = false;
            customerPortraitImage.gameObject.SetActive(true);
        }

        if (dialogueWindowRoot != null)
        {
            dialogueWindowRoot.SetActive(true);

            Image dialogueBoxImage = dialogueWindowRoot.GetComponent<Image>();
            if (dialogueBoxImage != null)
                dialogueBoxImage.raycastTarget = false;
        }

        ApplyDialogueLayerOrder();

        if (dialogueText != null)
        {
            if (dialogueFont != null)
                dialogueText.font = dialogueFont;

            dialogueText.fontSize = 22f;
            dialogueText.color = Color.white;
            dialogueText.alignment = TextAlignmentOptions.Center;
            dialogueText.textWrappingMode = TextWrappingModes.Normal;
            dialogueText.raycastTarget = false;
            dialogueText.gameObject.SetActive(true);
        }
    }

    private void ApplyDialogueLayerOrder()
    {
        if (viewFrontRoot == null)
            return;

        Transform deliveryWindow = viewFrontRoot.Find("DeliveryWindow_Prefab");
        if (deliveryWindow == null)
            return;

        int deliveryIndex = deliveryWindow.GetSiblingIndex();

        if (customerPortraitImage != null)
            customerPortraitImage.transform.SetSiblingIndex(Mathf.Max(0, deliveryIndex - 2));

        deliveryIndex = deliveryWindow.GetSiblingIndex();

        if (dialogueWindowRoot != null)
            dialogueWindowRoot.transform.SetSiblingIndex(Mathf.Max(0, deliveryIndex - 1));
    }

    private void DisableGameplayBehaviours()
    {
        if (viewFrontRoot == null)
            return;

        List<MonoBehaviour> behavioursToDisable = new List<MonoBehaviour>();

        foreach (IssueWindowCustomerPresenter presenter in viewFrontRoot.GetComponentsInChildren<IssueWindowCustomerPresenter>(true))
        {
            if (presenter != null)
                behavioursToDisable.Add(presenter);
        }

        foreach (BlindsToggle blinds in viewFrontRoot.GetComponentsInChildren<BlindsToggle>(true))
        {
            if (blinds != null)
                behavioursToDisable.Add(blinds);
        }

        foreach (OrderCompleteBtn orderButton in viewFrontRoot.GetComponentsInChildren<OrderCompleteBtn>(true))
        {
            if (orderButton != null)
                behavioursToDisable.Add(orderButton);
        }

        foreach (ThreatSpawner2 threatSpawner in viewFrontRoot.GetComponentsInChildren<ThreatSpawner2>(true))
        {
            if (threatSpawner != null)
                behavioursToDisable.Add(threatSpawner);
        }

        foreach (MonoBehaviour behaviour in behavioursToDisable)
            behaviour.enabled = false;
    }

    private void ShowLine(int index)
    {
        currentLineIndex = index;
        if (dialogueText != null && index >= 0 && index < DialogueLines.Length)
            dialogueText.text = DialogueLines[index];
    }

    private void AdvanceDialogue()
    {
        int nextIndex = currentLineIndex + 1;
        if (nextIndex >= DialogueLines.Length)
        {
            StartGameplay();
            return;
        }

        ShowLine(nextIndex);
    }

    private void StartGameplay()
    {
        Time.timeScale = 1f;
        GameOverManager.Instance?.PrepareForNewSession();
        SceneManager.LoadScene(gameplaySceneIndex);

        if (GameStateMachine.Instance != null)
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Gameplay);
    }
}
