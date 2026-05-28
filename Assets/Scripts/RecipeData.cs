using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostbiteKitchen.Data
{
    [Serializable]
    public struct IngredientRequirement
    {
        [Tooltip("Перетащи сюда нужный ScriptableObject ингредиента")]
        public IngredientData ingredient;
        
        [Tooltip("Сколько штук требуется")]
        public int count;
    }

    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Frostbite Kitchen/Data/Recipe")]
    public class RecipeData : ScriptableObject
    {
        [Header("Recipe Identity")]
        public string recipeId;
        public string recipeName;
        public Sprite icon;

        [Header("Requirements")]
        [Tooltip("Список необходимых ингредиентов и их количества для сборки блюда")]
        public List<IngredientRequirement> requiredIngredients;

        [Header("Balance Settings")]
        [Tooltip("Время в секундах, за которое игрок должен успеть отдать это блюдо")]
        public float timeLimit = 30f;
    }
}