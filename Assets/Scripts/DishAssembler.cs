using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;
public class DishAssembler : MonoBehaviour
{
    [Header("Current Plate Status")]
    [SerializeField] private List<IngredientData> ingredientsOnPlate = new();

    public static System.Action<List<IngredientData>> OnPlateUpdated;
    public static System.Action OnPlateCleared;
    public static DishAssembler Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void AddIngredient(IngredientData newIngredient)
    {
        if (newIngredient == null) return;
        ingredientsOnPlate.Add(newIngredient);
        Debug.Log($"[DishAssembler] Добавлен ингредиент: {newIngredient.displayName}. Всего на тарелке: {ingredientsOnPlate.Count}");
        OnPlateUpdated?.Invoke(ingredientsOnPlate);
    }
    public void ClearPlate()
    {
        ingredientsOnPlate.Clear();
        Debug.Log("[DishAssembler] Тарелка полностью очищена.");

        OnPlateCleared?.Invoke();
    }
    public void TrySubmitOrder()
    {
        if (OrderManager.Instance == null) return;

        RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();

        if (activeRecipe == null)
        {
            Debug.LogWarning("[DishAssembler] Нет активного заказа для сдачи!");
            return;
        }

        if (ValidateRecipe(activeRecipe))
        {
            Debug.Log($"[DishAssembler] Успех! Блюдо {activeRecipe.recipeName} собрано правильно.");

            OrderManager.Instance.CompleteActiveOrder();

            ClearPlate();
        }
        else
        {
            Debug.Log("[DishAssembler] Ошибка! Состав на тарелке не совпадает с рецептом.");
        }
    }
    private bool ValidateRecipe(RecipeData recipe)
    {
        Dictionary<string, int> requiredCounts = new();
        foreach (var req in recipe.requiredIngredients)
        {
            if (req.ingredient != null)
            {
                requiredCounts[req.ingredient.ingredientId] = req.count;
            }
        }

        Dictionary<string, int> currentCounts = new();
        foreach (var ing in ingredientsOnPlate)
        {
            if (ing != null)
            {
                if (currentCounts.ContainsKey(ing.ingredientId))
                    currentCounts[ing.ingredientId]++;
                else
                    currentCounts[ing.ingredientId] = 1;
            }
        }

        if (requiredCounts.Count != currentCounts.Count) return false;

        foreach (var pair in requiredCounts)
        {
            string ingId = pair.Key;
            int reqCount = pair.Value;

            if (!currentCounts.ContainsKey(ingId) || currentCounts[ingId] != reqCount)
            {
                return false;
            }
        }

        return true;
    }
}