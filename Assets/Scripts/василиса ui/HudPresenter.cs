using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrostbiteKitchen.Data;

public class HudPresenter : MonoBehaviour
{
    [SerializeField] private GameObject orderCardPrefab;
    [SerializeField] private Transform orderContainer;
    [SerializeField] private TextMeshProUGUI textNumberPeople;

    private GameObject currentOrderCard;
    private Image imageOrder;
    private TextMeshProUGUI textOrderTime;

    private float currentOrderTimer;
    private bool isOrderActive;
    private int servedCount = 0;
    private int totalOrdersTarget = 10;

    private void OnEnable()
    {
        Debug.Log("HudPresenter: Скрипт включен и подписан на события.");
        OrderManager.OnNewOrderStarted += ShowNewOrder;
        OrderManager.OnOrderExpired += HandleOrderExpired;
    }

    private void OnDisable()
    {
        OrderManager.OnNewOrderStarted -= ShowNewOrder;
        OrderManager.OnOrderExpired -= HandleOrderExpired;
    }

    private void Start()
    {
        Debug.Log("HudPresenter: Метод Start.");
        UpdateCustomersUi();

        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveRecipe() != null)
        {
            Debug.Log("HudPresenter: Обнаружен активный заказ при старте, запускаю отображение вручную.");
            ShowNewOrder(OrderManager.Instance.GetActiveRecipe());
        }
    }

    private void Update()
    {
        if (!isOrderActive) return;

        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveRecipe() == null)
        {
            servedCount++;
            UpdateCustomersUi();
            HandleOrderExpired();
            return;
        }

        currentOrderTimer -= Time.deltaTime;
        if (currentOrderTimer < 0) currentOrderTimer = 0;

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
        {
            Debug.LogError("HudPresenter: Ошибка! Не задан префаб или контейнер.");
            return;
        }

        if (currentOrderCard != null) Destroy(currentOrderCard);

        currentOrderCard = Instantiate(orderCardPrefab, orderContainer);

        Transform imageTransform = currentOrderCard.transform.Find("Image_Order");
        if (imageTransform != null)
        {
            imageOrder = imageTransform.GetComponent<Image>();
        }
        else
        {
            imageOrder = currentOrderCard.GetComponentInChildren<Image>();
        }

        textOrderTime = currentOrderCard.GetComponentInChildren<TextMeshProUGUI>();

        if (newRecipe.icon != null && imageOrder != null) imageOrder.sprite = newRecipe.icon;
        
        currentOrderTimer = newRecipe.timeLimit;
        isOrderActive = true;
    }

    private void HandleOrderExpired()
    {
        Debug.Log("HudPresenter: Заказ завершен/истек.");
        isOrderActive = false;
        if (currentOrderCard != null) Destroy(currentOrderCard);
    }

    private void UpdateCustomersUi()
    {
        textNumberPeople.text = $"{servedCount}/{totalOrdersTarget}";
    }
}