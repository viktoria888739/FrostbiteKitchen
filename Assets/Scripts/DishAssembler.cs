<<<<<<< Updated upstream
=======
using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;
<<<<<<< Updated upstream
using FrostbiteKitchen.Gameplay;

public class DishAssembler : MonoBehaviour
=======
using FrostbiteKitchen.KitchenStation;

namespace FrostbiteKitchen.Gameplay
>>>>>>> Stashed changes
{
    public static DishAssembler Instance { get; private set; }

    [Header("Current Plate Status")]
    [SerializeField] private List<IngredientData> ingredientsOnPlate = new();

    [Header("State Saving & Safety")]
    [SerializeField] private List<IngredientData> frozenIngredientsBuffer = new();
    [SerializeField] private bool isFrozen = false;

    [Header("Events")]
    public static System.Action<List<IngredientData>> OnDishChanged;
    public static System.Action OnPlateCleared;

    private void Awake()
    {
<<<<<<< Updated upstream
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
=======
        public static DishAssembler Instance { get; private set; }

        [SerializeField] private int maxIngredientsOnPlate = 5;
        [SerializeField] private RecipeCatalog recipeCatalog;

        private readonly List<IngredientData> ingredientsOnPlate = new List<IngredientData>();
        private IngredientData[] frozenIngredientsBackup;
        private bool isInteractionFrozen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ResolveRecipeCatalog();
>>>>>>> Stashed changes
        }
        Instance = this;

        ingredientsOnPlate ??= new List<IngredientData>();
        frozenIngredientsBuffer ??= new List<IngredientData>();
    }

    private void OnEnable()
    {
        GameStateMachine.OnStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.OnStateChanged -= HandleGameStateChanged;
    }

    public int GetCurrentIngredientCount()
    {
        return ingredientsOnPlate?.Count ?? 0;
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        if (newState != GameStateMachine.GameState.Gameplay)
        {
<<<<<<< Updated upstream
            FreezeCurrentState();
=======
            GameStateMachine.OnStateChanged += HandleGameStateChanged;
            SyncFreezeStateWithGame();
>>>>>>> Stashed changes
        }
        else
        {
<<<<<<< Updated upstream
            ResumeCurrentState();
        }
    }

    public void FreezeCurrentState()
    {
        if (isFrozen) return;

        frozenIngredientsBuffer ??= new List<IngredientData>();
        frozenIngredientsBuffer.Clear();

        if (ingredientsOnPlate != null)
        {
            foreach (var ingredient in ingredientsOnPlate)
            {
                if (ingredient != null)
                    frozenIngredientsBuffer.Add(ingredient);
            }
        }

        isFrozen = true;
        Debug.Log($"[DishAssembler] Состояние ЗАМОРОЖЕНО. В буфер сохранено: {frozenIngredientsBuffer.Count} шт.");
    }

    public void ResumeCurrentState()
    {
        if (!isFrozen) return;

        ingredientsOnPlate ??= new List<IngredientData>();
        ingredientsOnPlate.Clear();

        if (frozenIngredientsBuffer != null)
        {
            foreach (var ingredient in frozenIngredientsBuffer)
            {
                if (ingredient != null)
                    ingredientsOnPlate.Add(ingredient);
            }
        }

        isFrozen = false;
        Debug.Log($"[DishAssembler] Состояние ВОССТАНОВЛЕНО. На тарелке снова: {ingredientsOnPlate.Count} шт.");

        OnDishChanged?.Invoke(ingredientsOnPlate);
    }

    public void AddIngredient(IngredientData newIngredient)
    {
        if (newIngredient == null)
        {
            Debug.LogWarning("[DishAssembler] Попытка добавить пустой ингредиент (null)!");
            return;
        }

        if (isFrozen)
        {
            frozenIngredientsBuffer ??= new List<IngredientData>();
            frozenIngredientsBuffer.Add(newIngredient);
            Debug.Log($"[DishAssembler] Добавлен в замороженный буфер: {newIngredient.displayName}");
            return;
        }

        ingredientsOnPlate ??= new List<IngredientData>();
        ingredientsOnPlate.Add(newIngredient);

        Debug.Log($"[DishAssembler] Ингредиент '{newIngredient.displayName}' добавлен. Всего: {ingredientsOnPlate.Count}");

        OnDishChanged?.Invoke(ingredientsOnPlate);
    }

    public void ClearPlate()
    {
        ingredientsOnPlate?.Clear();
        frozenIngredientsBuffer?.Clear();
        isFrozen = false;

        Debug.Log("[DishAssembler] Тарелка полностью очищена.");

        OnDishChanged?.Invoke(ingredientsOnPlate);
        OnPlateCleared?.Invoke();
    }
    public bool ValidateRecipe(RecipeData targetRecipe)
    {
        if (targetRecipe == null || targetRecipe.requiredIngredients == null)
            return false;

        var required = new Dictionary<IngredientData, int>();
        foreach (var req in targetRecipe.requiredIngredients)
        {
            if (req.ingredient != null)
                required[req.ingredient] = required.GetValueOrDefault(req.ingredient) + req.count;
        }

        var current = new Dictionary<IngredientData, int>();
        foreach (var ing in ingredientsOnPlate)
        {
            if (ing != null)
                current[ing] = current.GetValueOrDefault(ing) + 1;
        }

        if (required.Count != current.Count)
            return false;

        foreach (var pair in required)
        {
            if (!current.TryGetValue(pair.Key, out int count) || count != pair.Value)
                return false;
        }

        return true;
    }

    [ContextMenu("Force Freeze State")]
    private void DebugFreeze() => FreezeCurrentState();

    [ContextMenu("Force Resume State")]
    private void DebugResume() => ResumeCurrentState();

    [ContextMenu("Clear Plate")]
    private void DebugClear() => ClearPlate();
}
=======
            GameStateMachine.OnStateChanged -= HandleGameStateChanged;
        }

        public bool TryAddIngredient(IngredientData ingredient)
        {
            if (isInteractionFrozen || ingredient == null)
                return false;

            if (ingredientsOnPlate.Count >= maxIngredientsOnPlate)
                return false;

            ingredientsOnPlate.Add(ingredient);
            UpdatePlateVisuals();
            return true;
        }

        public bool HasCompleteDish()
        {
            return TryResolveCompleteRecipe(out _);
        }

        public bool TryBuildCompleteDish(out DishData dish)
        {
            dish = null;

            if (isInteractionFrozen || !TryResolveCompleteRecipe(out RecipeData recipe))
                return false;

            dish = ScriptableObject.CreateInstance<DishData>();
            dish.dishId = recipe.recipeId;
            dish.dishName = recipe.recipeName;
            dish.icon = recipe.icon;
            dish.correspondingRecipe = recipe;
            dish.ingredients = new List<IngredientData>(ingredientsOnPlate);
            return true;
        }

        public bool TryPickupCompleteDish(out DishData dish)
        {
            if (!TryBuildCompleteDish(out dish))
                return false;

            ClearPlate();
            return true;
        }

        public void ClearPlate()
        {
            ingredientsOnPlate.Clear();
            frozenIngredientsBackup = null;
            UpdatePlateVisuals();
        }

        public void AddIngredient(IngredientData ingredient)
        {
            TryAddIngredient(ingredient);
        }

        public int GetCurrentIngredientCount()
        {
            return ingredientsOnPlate.Count;
        }

        public bool ValidateRecipe(RecipeData recipe)
        {
            return ValidateIngredients(ingredientsOnPlate, recipe);
        }

        public static bool ValidateDish(DishData dish, RecipeData recipe)
        {
            if (dish == null || recipe == null)
                return false;

            if (dish.correspondingRecipe != null)
                return dish.correspondingRecipe == recipe;

            return ValidateIngredients(dish.ingredients, recipe);
        }

        public static bool ValidateIngredients(IReadOnlyList<IngredientData> ingredients, RecipeData recipe)
        {
            if (recipe == null || recipe.requiredIngredients == null)
                return false;

            Dictionary<IngredientData, int> requiredCounts = BuildRequiredCounts(recipe);
            Dictionary<IngredientData, int> providedCounts = BuildIngredientCounts(ingredients);

            if (requiredCounts.Count != providedCounts.Count)
                return false;

            foreach (KeyValuePair<IngredientData, int> requirement in requiredCounts)
            {
                if (!providedCounts.TryGetValue(requirement.Key, out int count) || count != requirement.Value)
                    return false;
            }

            return true;
        }

        public List<IngredientData> GetCurrentIngredients()
        {
            return new List<IngredientData>(ingredientsOnPlate);
        }

        private bool TryResolveCompleteRecipe(out RecipeData recipe)
        {
            recipe = null;

            if (ingredientsOnPlate.Count == 0)
                return false;

            Dictionary<IngredientData, int> plateCounts = BuildIngredientCounts(ingredientsOnPlate);
            IReadOnlyList<RecipeData> recipes = GetRecipes();
            if (recipes == null)
                return false;

            foreach (RecipeData candidate in recipes)
            {
                if (candidate == null)
                    continue;

                Dictionary<IngredientData, int> requiredCounts = BuildRequiredCounts(candidate);
                if (CountsMatchExactly(plateCounts, requiredCounts))
                {
                    recipe = candidate;
                    return true;
                }
            }

            return false;
        }

        private IReadOnlyList<RecipeData> GetRecipes()
        {
            if (recipeCatalog == null)
                ResolveRecipeCatalog();

            return recipeCatalog != null ? recipeCatalog.AllRecipes : null;
        }

        private void ResolveRecipeCatalog()
        {
            if (recipeCatalog != null)
                return;

            recipeCatalog = Resources.Load<RecipeCatalog>("MainRecipeCatalog");
        }

        private void HandleGameStateChanged(GameStateMachine.GameState newState)
        {
            if (newState != GameStateMachine.GameState.Gameplay)
                FreezeAssemblerState();
            else
                UnfreezeAssemblerState();
        }

        private void SyncFreezeStateWithGame()
        {
            if (GameStateMachine.Instance == null)
                return;

            HandleGameStateChanged(GameStateMachine.Instance.CurrentState);
        }

        private void FreezeAssemblerState()
        {
            isInteractionFrozen = true;

            if (ingredientsOnPlate.Count > 0)
                frozenIngredientsBackup = ingredientsOnPlate.ToArray();
        }

        private void UnfreezeAssemblerState()
        {
            isInteractionFrozen = false;

            if (frozenIngredientsBackup == null)
                return;

            ingredientsOnPlate.Clear();
            ingredientsOnPlate.AddRange(frozenIngredientsBackup);
            frozenIngredientsBackup = null;
            UpdatePlateVisuals();
        }

        private void UpdatePlateVisuals()
        {
            AssemblyTable.Instance?.ResetTable();
        }

        private static Dictionary<IngredientData, int> BuildIngredientCounts(IReadOnlyList<IngredientData> ingredients)
        {
            Dictionary<IngredientData, int> counts = new Dictionary<IngredientData, int>();

            if (ingredients == null)
                return counts;

            foreach (IngredientData ingredient in ingredients)
            {
                if (ingredient == null)
                    continue;

                counts.TryGetValue(ingredient, out int amount);
                counts[ingredient] = amount + 1;
            }

            return counts;
        }

        private static Dictionary<IngredientData, int> BuildRequiredCounts(RecipeData recipe)
        {
            Dictionary<IngredientData, int> counts = new Dictionary<IngredientData, int>();

            if (recipe.requiredIngredients == null)
                return counts;

            foreach (IngredientRequirement requirement in recipe.requiredIngredients)
            {
                if (requirement.ingredient == null || requirement.count <= 0)
                    continue;

                counts.TryGetValue(requirement.ingredient, out int amount);
                counts[requirement.ingredient] = amount + requirement.count;
            }

            return counts;
        }

        private static bool CountsMatchExactly(
            Dictionary<IngredientData, int> providedCounts,
            Dictionary<IngredientData, int> requiredCounts)
        {
            if (providedCounts.Count != requiredCounts.Count)
                return false;

            foreach (KeyValuePair<IngredientData, int> requirement in requiredCounts)
            {
                if (!providedCounts.TryGetValue(requirement.Key, out int count) || count != requirement.Value)
                    return false;
            }

            return true;
        }
    }
}
>>>>>>> Stashed changes
