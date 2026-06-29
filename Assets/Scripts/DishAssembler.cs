using System.Collections.Generic;
using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.KitchenStation;

namespace FrostbiteKitchen.Gameplay
{
    public class DishAssembler : MonoBehaviour
    {
        public static DishAssembler Instance { get; private set; }

        [SerializeField] private int maxIngredientsOnPlate = 5;
        [SerializeField] private RecipeCatalog recipeCatalog;
        [SerializeField] private Sprite spoiledDishSprite;

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
        }

        private void OnEnable()
        {
            GameStateMachine.OnStateChanged += HandleGameStateChanged;
            SyncFreezeStateWithGame();
        }

        private void OnDisable()
        {
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

        public bool HasIngredientsOnPlate()
        {
            return ingredientsOnPlate.Count > 0;
        }

        public bool IsCurrentPlateSpoiled()
        {
            if (ingredientsOnPlate.Count == 0)
                return false;

            IReadOnlyList<RecipeData> recipes = GetRecipes();
            return recipes == null ||
                   AssemblyPlateVisualResolver.IsSpoiledPlate(ingredientsOnPlate, recipes);
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
            if (!TryCreateDishFromPlate(out dish))
                return false;

            ClearPlate();
            return true;
        }

        public bool TryPickupPlateDish(out DishData dish)
        {
            return TryPickupCompleteDish(out dish);
        }

        public bool TryCreateDishFromPlate(out DishData dish)
        {
            dish = null;

            if (isInteractionFrozen || ingredientsOnPlate.Count == 0)
                return false;

            dish = ScriptableObject.CreateInstance<DishData>();
            dish.ingredients = new List<IngredientData>(ingredientsOnPlate);

            if (TryResolveCompleteRecipe(out RecipeData recipe))
            {
                dish.dishId = recipe.recipeId;
                dish.dishName = recipe.recipeName;
                dish.icon = recipe.icon;
                dish.correspondingRecipe = recipe;
            }
            else
            {
                dish.dishId = "spoiled_dish";
                dish.dishName = "Испорченное блюдо";
                dish.icon = ResolveSpoiledSprite();
                dish.correspondingRecipe = null;
            }

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

        private Sprite ResolveSpoiledSprite()
        {
            if (spoiledDishSprite != null)
                return spoiledDishSprite;

            if (AssemblyTable.Instance != null)
            {
                Sprite fromTable = AssemblyTable.Instance.GetSpoiledDishSprite();
                if (fromTable != null)
                    return fromTable;
            }

            return null;
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
