using UnityEngine;

namespace FrostbiteKitchen.Data
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Kitchen/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;

        [Header("Настройки готовки/жарки")]
        [SerializeField] private bool requiresCooking;
        [SerializeField] private float cookingTime = 3f;
        [SerializeField] private IngredientData cookedVersion;

        // Геттеры для доступа из скрипта плиты
        public bool RequiresCooking => requiresCooking;
        public float CookingTime => cookingTime;
        public IngredientData CookedVersion => cookedVersion;
    }
}