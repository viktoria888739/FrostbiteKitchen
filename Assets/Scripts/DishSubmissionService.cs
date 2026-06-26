using UnityEngine;
using FrostbiteKitchen.Data;

public class DishSubmissionService : MonoBehaviour
{
    public static DishSubmissionService Instance { get; private set; }

    public static System.Action OnOrderSubmitted;
    public static System.Action OnOrderFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void TrySubmitDish()
    {
        if (OrderManager.Instance == null || DishAssembler.Instance == null)
        {
            Debug.LogError("[DishSubmissionService] Не найдены необходимые менеджеры!");
            return;
        }

        RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
        if (activeRecipe == null)
        {
            Debug.LogWarning("[DishSubmissionService] Нет активного заказа.");
            return;
        }

        bool isCorrect = DishAssembler.Instance.ValidateRecipe(activeRecipe);

        if (isCorrect)
        {
            OrderManager.Instance.CompleteActiveOrder();
            DishAssembler.Instance.ClearPlate();
            OnOrderSubmitted?.Invoke();
            Debug.Log("<color=green>[DishSubmissionService]</color> Заказ успешно сдан!");
        }
        else
        {
            OrderManager.Instance.FailCurrentOrder();
            DishAssembler.Instance.ClearPlate();
            OnOrderFailed?.Invoke();
            Debug.Log("<color=red>[DishSubmissionService]</color> Заказ провален (неверный состав).");
        }
    }
}