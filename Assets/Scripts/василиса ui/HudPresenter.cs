using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrostbiteKitchen.Data;

public class HudPresenter : MonoBehaviour
{
    [SerializeField] private GameObject panelOrder;
    [SerializeField] private TextMeshProUGUI textOrderTime;
    [SerializeField] private Image imageOrder;
    [SerializeField] private TextMeshProUGUI textNumberPeople;

    private float currentOrderTimer;
    private bool isOrderActive;
    private int servedCount = 0;
    private int totalOrdersTarget = 10;

    private void OnEnable()
    {
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
        UpdateCustomersUi();
        panelOrder.SetActive(false);
    }

    private void Update()
    {
        if (!isOrderActive) return;

        if (OrderManager.Instance != null && OrderManager.Instance.GetActiveRecipe() == null)
        {
            if (isOrderActive)
            {
                servedCount++;
                UpdateCustomersUi();
            }
            HandleOrderExpired();
            return;
        }

        currentOrderTimer -= Time.deltaTime;
        if (currentOrderTimer < 0)
        {
            currentOrderTimer = 0;
        }

        int minutes = Mathf.FloorToInt(currentOrderTimer / 60f);
        int seconds = Mathf.FloorToInt(currentOrderTimer % 60f);
        textOrderTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ShowNewOrder(RecipeData newRecipe)
    {
        currentOrderTimer = newRecipe.timeLimit;
        
        if (newRecipe.icon != null)
        {
            imageOrder.sprite = newRecipe.icon;
        }
        
        isOrderActive = true;
        panelOrder.SetActive(true);
    }

    private void HandleOrderExpired()
    {
        isOrderActive = false;
        panelOrder.SetActive(false);
    }

    private void UpdateCustomersUi()
    {
        textNumberPeople.text = $"{servedCount}/{totalOrdersTarget}";
    }

    public void OnRecipeListToggle()
    {
        Debug.Log("Список рецептов");
    }

    public void OnPauseToggle()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.TogglePause();
        }
    }
}