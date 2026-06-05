using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;

public class DishAssembler : MonoBehaviour
{
    [Header("Current Plate Status")]
    [SerializeField] private List<IngredientData> ingredientsOnPlate = new();
    
    // Новое: текущее собранное блюдо
    public DishData CurrentDish { get; private set; }

    public static System.Action<List<IngredientData>> OnPlateUpdated;
    public static System.Action<DishData> OnDishUpdated;     // Новое событие
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

    public int GetCurrentIngredientCount() => ingredientsOnPlate.Count;

    public void AddIngredient(IngredientData newIngredient)
    {
        if (newIngredient == null) return;

        ingredientsOnPlate.Add(newIngredient);
        
        Debug.Log($"[DishAssembler] Добавлен: {newIngredient.displayName}. Всего: {ingredientsOnPlate.Count}");

        OnPlateUpdated?.Invoke(ingredientsOnPlate);
        TryFormDish(); // Пытаемся сформировать блюдо
    }

    // Пытаемся сопоставить текущие ингредиенты с каким-то рецептом
    private void TryFormDish()
    {
        if (OrderManager.Instance == null) return;

        RecipeData activeRecipe = OrderManager.Instance.GetActiveRecipe();
        if (activeRecipe == null) return;

        if (ValidateRecipe(activeRecipe))
        {
            // Создаём "блюдо" из текущих ингредиентов
            CurrentDish = ScriptableObject.CreateInstance<DishData>();
            CurrentDish.dishName = activeRecipe.recipeName + " (Assembled)";
            CurrentDish.ingredients = new List<IngredientData>(ingredientsOnPlate);
            CurrentDish.correspondingRecipe = activeRecipe;

            OnDishUpdated?.Invoke(CurrentDish);
            Debug.Log($"<color=cyan>[DishAssembler] Блюдо сформировано: {CurrentDish.dishName}</color>");
        }
    }

    public bool ValidateRecipe(RecipeData recipe)
    {
        if (recipe == null || recipe.requiredIngredients == null)
            return false;

        Dictionary<string, int> requiredCounts = new Dictionary<string, int>();
        foreach (var req in recipe.requiredIngredients)
        {
            if (req.ingredient != null && !string.IsNullOrEmpty(req.ingredient.id))
            {
                string id = req.ingredient.id;
                requiredCounts[id] = requiredCounts.ContainsKey(id) ? requiredCounts[id] + req.count : req.count;
            }
        }

        Dictionary<string, int> currentCounts = new Dictionary<string, int>();
        foreach (var ing in ingredientsOnPlate)
        {
            if (ing != null && !string.IsNullOrEmpty(ing.id))
            {
                string id = ing.id;
                currentCounts[id] = currentCounts.ContainsKey(id) ? currentCounts[id] + 1 : 1;
            }
        }

        if (requiredCounts.Count != currentCounts.Count)
            return false;

        foreach (var pair in requiredCounts)
        {
            if (!currentCounts.TryGetValue(pair.Key, out int count) || count != pair.Value)
                return false;
        }

        return true;
    }

    public void ClearPlate()
    {
        ingredientsOnPlate.Clear();
        CurrentDish = null;

        Debug.Log("[DishAssembler] Тарелка очищена.");
        OnPlateCleared?.Invoke();
    }
}