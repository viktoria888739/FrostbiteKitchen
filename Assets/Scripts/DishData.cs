using UnityEngine;
using System.Collections.Generic;

namespace FrostbiteKitchen.Data
{
    [CreateAssetMenu(fileName = "NewDish", menuName = "Frostbite Kitchen/Data/Dish")]
    public class DishData : ScriptableObject
    {
        [Header("Dish Identity")]
        public string dishId;
        public string dishName;
        public Sprite icon;

        [Header("Composition")]
        public List<IngredientData> ingredients = new List<IngredientData>();

        [Header("Result")]
        public RecipeData correspondingRecipe;  
    }
}