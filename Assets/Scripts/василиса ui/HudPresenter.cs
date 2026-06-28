using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrostbiteKitchen.Data;

public class HudPresenter : MonoBehaviour
{
    private const string OrderIconChildName = "Image_Order";
    private const string OrderTimerChildName = "Text_Order_Time";
    private const string CustomersCounterName = "Text_Number_People";

    [SerializeField] private GameObject orderCardPrefab;
    [SerializeField] private Transform orderContainer;
    [SerializeField] private TextMeshProUGUI textNumberPeople;

    private GameObject currentOrderCard;
    private Image imageOrder;
    private TextMeshProUGUI textOrderTime;

    private float currentOrderTimer;
    private bool isOrderActive;

    private void Awake()
    {
        ResolveCustomersCounterText();
    }

    private void OnEnable()
    {
        OrderManager.OnNewOrderStarted += ShowNewOrder;
        OrderManager.OnOrderExpired += HandleOrderFinished;
        OrderManager.OnOrderSubmitted += HandleOrderFinished;
        SessionOrderTracker.OnCustomerCountChanged += UpdateCustomersUi;
        RefreshCustomersCounter();
    }

    private void OnDisable()
    {
        OrderManager.OnNewOrderStarted -= ShowNewOrder;
        OrderManager.OnOrderExpired -= HandleOrderFinished;
        OrderManager.OnOrderSubmitted -= HandleOrderFinished;
        SessionOrderTracker.OnCustomerCountChanged -= UpdateCustomersUi;
    }

    private void Start()
    {
        ResolveCustomersCounterText();
        RefreshCustomersCounter();

        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveRecipe() != null)
        {
            ShowNewOrder(OrderManager.Instance.GetActiveRecipe());
        }
    }

    private void Update()
    {
        if (!isOrderActive) return;

        currentOrderTimer -= Time.deltaTime;
        if (currentOrderTimer < 0f) currentOrderTimer = 0f;

        if (textOrderTime != null)
        {
            int minutes = Mathf.FloorToInt(currentOrderTimer / 60f);
            int seconds = Mathf.FloorToInt(currentOrderTimer % 60f);
            textOrderTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void ShowNewOrder(RecipeData newRecipe)
    {
        if (orderCardPrefab == null || orderContainer == null)
            return;

        if (currentOrderCard != null) Destroy(currentOrderCard);

        currentOrderCard = Instantiate(orderCardPrefab, orderContainer);
        StretchCardToContainer(currentOrderCard);

        imageOrder = FindOrderImage(currentOrderCard.transform);
        textOrderTime = FindOrderTimer(currentOrderCard.transform);

        if (imageOrder != null)
        {
            imageOrder.preserveAspect = true;
            imageOrder.color = Color.white;

            if (newRecipe.icon != null)
                imageOrder.sprite = newRecipe.icon;
        }

        if (textOrderTime != null)
        {
            textOrderTime.enableAutoSizing = true;
            textOrderTime.fontSizeMin = 18f;
            textOrderTime.fontSizeMax = 42f;
        }

        currentOrderTimer = newRecipe.timeLimit;
        isOrderActive = true;
    }

    private static void StretchCardToContainer(GameObject card)
    {
        RectTransform rect = card.GetComponent<RectTransform>();
        if (rect == null) return;

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
    }

    private static Image FindOrderImage(Transform cardRoot)
    {
        Transform iconTransform = cardRoot.Find(OrderIconChildName);
        if (iconTransform != null && iconTransform.TryGetComponent(out Image iconImage))
            return iconImage;

        Image[] images = cardRoot.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject != cardRoot.gameObject)
                return image;
        }

        return null;
    }

    private static TextMeshProUGUI FindOrderTimer(Transform cardRoot)
    {
        Transform timerTransform = cardRoot.Find(OrderTimerChildName);
        if (timerTransform != null && timerTransform.TryGetComponent(out TextMeshProUGUI timerText))
            return timerText;

        return cardRoot.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void HandleOrderFinished()
    {
        isOrderActive = false;
        if (currentOrderCard != null)
        {
            Destroy(currentOrderCard);
            currentOrderCard = null;
        }

        imageOrder = null;
        textOrderTime = null;
    }

    private void ResolveCustomersCounterText()
    {
        if (textNumberPeople != null)
            return;

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.gameObject.name == CustomersCounterName)
            {
                textNumberPeople = text;
                return;
            }
        }

        GameObject counterObject = GameObject.Find(CustomersCounterName);
        if (counterObject != null)
        {
            textNumberPeople = counterObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private void RefreshCustomersCounter()
    {
        if (SessionOrderTracker.Instance != null)
        {
            UpdateCustomersUi(
                SessionOrderTracker.Instance.CurrentOrdersCount,
                SessionOrderTracker.Instance.MaxOrders);
            return;
        }

        UpdateCustomersUi(0, 10);
    }

    private void UpdateCustomersUi(int servedCount, int totalOrdersTarget)
    {
        ResolveCustomersCounterText();
        if (textNumberPeople == null)
            return;

        textNumberPeople.text = $"{servedCount}/{totalOrdersTarget}";
    }
}
