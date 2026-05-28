using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject orderCardPrefab;
    [SerializeField] private Transform cardsContainer;

    private GameObject currentOrderCard;

    private void Awake()
    {
        Debug.Log("[OrderUIManager] Awake");
        if (orderCardPrefab == null) Debug.LogError("[OrderUIManager] orderCardPrefab не назначен в инспекторе!");
        if (cardsContainer == null) Debug.LogError("[OrderUIManager] cardsContainer не назначен в инспекторе!");
    }

    private void OnEnable()
    {
        Debug.Log("[OrderUIManager] OnEnable – подписка на события OrderManager");
        OrderManager.OnNewOrderStarted += OnNewOrderStarted;
        OrderManager.OnOrderExpired += OnOrderExpired;
    }

    private void OnDisable()
    {
        Debug.Log("[OrderUIManager] OnDisable – отписка от событий");
        OrderManager.OnNewOrderStarted -= OnNewOrderStarted;
        OrderManager.OnOrderExpired -= OnOrderExpired;
    }

    private void OnNewOrderStarted(RecipeData recipe)
    {
        Debug.Log($"[OrderUIManager] OnNewOrderStarted получен! Рецепт: {recipe.displayName}");

        if (currentOrderCard != null)
        {
            Debug.Log("[OrderUIManager] Удаляю старую карточку");
            Destroy(currentOrderCard);
        }

        if (orderCardPrefab == null)
        {
            Debug.LogError("[OrderUIManager] orderCardPrefab = null, не могу создать карточку!");
            return;
        }
        if (cardsContainer == null)
        {
            Debug.LogError("[OrderUIManager] cardsContainer = null, не могу создать карточку!");
            return;
        }

        Debug.Log("[OrderUIManager] Создаю новую карточку через Instantiate");
        currentOrderCard = Instantiate(orderCardPrefab, cardsContainer);
        Debug.Log($"[OrderUIManager] Карточка создана: {currentOrderCard.name}");

        OrderCard card = currentOrderCard.GetComponent<OrderCard>();
        if (card != null)
        {
            card.Setup(recipe);
            Debug.Log("[OrderUIManager] Setup карточки вызван");
        }
        else
        {
            Debug.LogError("[OrderUIManager] На префабе отсутствует компонент OrderCard!");
        }
    }

    private void OnOrderExpired()
    {
        Debug.Log("[OrderUIManager] OnOrderExpired – заказ просрочен, удаляю карточку");
        if (currentOrderCard != null)
        {
            Destroy(currentOrderCard);
            currentOrderCard = null;
        }
    }
}