using UnityEngine;

namespace FrostbiteKitchen.Data
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Kitchen/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
        [Header("Основные данные")]
        public string id;
        public string displayName;

        [Header("UI Иконки")]
        [Tooltip("Иконка, когда в руках 1 штука ингредиента (например, взят со стола)")]
        public Sprite icon;

        [Tooltip("Иконка, когда в руках пачка (взята со склада, количество > 1)")]
        public Sprite packIcon;

        [Header("Настройки готовки/жарки")]
        [SerializeField] private bool requiresCooking;
        [SerializeField] private float cookingTime = 3f;
        [SerializeField] private IngredientData cookedVersion;

        // Публичные свойства для безопасного чтения данных из других скриптов (например, со Stove.cs)
        public bool RequiresCooking => requiresCooking;
        public float CookingTime => cookingTime;
        public IngredientData CookedVersion => cookedVersion;
    }
}