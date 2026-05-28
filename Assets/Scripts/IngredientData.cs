using UnityEngine;

namespace FrostbiteKitchen.Data
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Frostbite Kitchen/Data/Ingredient")]
    public class IngredientData : ScriptableObject
    {
        [Header("System Identifiers")]
        [Tooltip("Уникальный ID для кода и логики сборки блюд (например: venison, meat_raw)")]
        public string ingredientId;

        [Header("Display Settings")]
        [Tooltip("Название для игрока на русском, которое Василиса выведет в интерфейс")]
        public string displayName; 

        [Tooltip("Спрайт, который нарисует Эллина")]
        public Sprite icon; 

        [Tooltip("Цвет-заглушка, если Эллина еще не отдала финальный арт")]
        public Color debugColor = Color.white; 
    }
}