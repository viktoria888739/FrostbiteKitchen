using System.Collections.Generic;
using UnityEngine;

namespace FrostbiteKitchen.Data
{
    [CreateAssetMenu(fileName = "NewRecipeCatalog", menuName = "Frostbite Kitchen/Data/Recipe Catalog")]
    public class RecipeCatalog : ScriptableObject
    {
        [Header("All Game Recipes")]
        [Tooltip("Перетащитть сюда абсолютно все ScriptableObject-ы рецептов, которые есть в игре")]
        [SerializeField] private List<RecipeData> allRecipes = new();

        public List<RecipeData> AllRecipes => allRecipes;
        public RecipeData GetRecipeById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            foreach (var recipe in allRecipes)
            {
                if (recipe != null && recipe.recipeId == id)
                {
                    return recipe;
                }
            }

            Debug.LogWarning($"[RecipeCatalog] Рецепт с ID '{id}' не найден в каталоге!");
            return null;
        }
    }
}