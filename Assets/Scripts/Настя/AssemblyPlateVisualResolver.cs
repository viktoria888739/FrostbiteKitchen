using System.Collections.Generic;
using FrostbiteKitchen.Data;
using UnityEngine;

namespace FrostbiteKitchen.KitchenStation
{
    public static class AssemblyPlateVisualResolver
    {
        public static Sprite ResolvePlateSprite(
            IReadOnlyList<IngredientData> plateIngredients,
            IReadOnlyList<RecipeData> recipes,
            Sprite spoiledDishSprite)
        {
            if (plateIngredients == null || plateIngredients.Count == 0)
                return null;

            if (plateIngredients.Count == 1)
                return plateIngredients[0] != null ? plateIngredients[0].icon : null;

            Dictionary<IngredientData, int> plateCounts = BuildCounts(plateIngredients);

            RecipeData exactRecipe = FindExactRecipe(plateCounts, recipes);
            if (exactRecipe != null)
                return exactRecipe.icon;

            if (plateIngredients.Count == 2)
            {
                if (IsPrefixOfAnyRecipe(plateCounts, recipes))
                {
                    IngredientData lastIngredient = plateIngredients[plateIngredients.Count - 1];
                    return lastIngredient != null ? lastIngredient.icon : null;
                }

                return spoiledDishSprite;
            }

            return spoiledDishSprite;
        }

        private static RecipeData FindExactRecipe(
            Dictionary<IngredientData, int> plateCounts,
            IReadOnlyList<RecipeData> recipes)
        {
            if (recipes == null)
                return null;

            foreach (RecipeData recipe in recipes)
            {
                if (recipe == null)
                    continue;

                Dictionary<IngredientData, int> requiredCounts = BuildRequiredCounts(recipe);
                if (ExactMatch(plateCounts, requiredCounts))
                    return recipe;
            }

            return null;
        }

        private static bool IsPrefixOfAnyRecipe(
            Dictionary<IngredientData, int> plateCounts,
            IReadOnlyList<RecipeData> recipes)
        {
            if (recipes == null)
                return false;

            foreach (RecipeData recipe in recipes)
            {
                if (recipe == null)
                    continue;

                Dictionary<IngredientData, int> requiredCounts = BuildRequiredCounts(recipe);
                if (IsSubsetAndIncomplete(plateCounts, requiredCounts))
                    return true;
            }

            return false;
        }

        private static Dictionary<IngredientData, int> BuildCounts(IReadOnlyList<IngredientData> ingredients)
        {
            Dictionary<IngredientData, int> counts = new Dictionary<IngredientData, int>();

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

        private static bool ExactMatch(
            Dictionary<IngredientData, int> plateCounts,
            Dictionary<IngredientData, int> requiredCounts)
        {
            if (plateCounts.Count != requiredCounts.Count)
                return false;

            foreach (KeyValuePair<IngredientData, int> requirement in requiredCounts)
            {
                if (!plateCounts.TryGetValue(requirement.Key, out int amount) || amount != requirement.Value)
                    return false;
            }

            return true;
        }

        private static bool IsSubsetAndIncomplete(
            Dictionary<IngredientData, int> plateCounts,
            Dictionary<IngredientData, int> requiredCounts)
        {
            int plateTotal = 0;
            int requiredTotal = 0;

            foreach (KeyValuePair<IngredientData, int> requirement in requiredCounts)
                requiredTotal += requirement.Value;

            foreach (KeyValuePair<IngredientData, int> plateEntry in plateCounts)
            {
                plateTotal += plateEntry.Value;

                if (!requiredCounts.TryGetValue(plateEntry.Key, out int requiredAmount))
                    return false;

                if (plateEntry.Value > requiredAmount)
                    return false;
            }

            return plateTotal > 0 && plateTotal < requiredTotal;
        }
    }
}
