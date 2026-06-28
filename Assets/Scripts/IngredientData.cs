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

<<<<<<< Updated upstream
=======
        [Header("Настройки нарезки")]
        [SerializeField] private bool requiresCutting;
        [SerializeField] private float cuttingTime = 2f;
        [SerializeField] private IngredientData cutVersion;

>>>>>>> Stashed changes
        public bool RequiresCooking => requiresCooking;
        public float CookingTime => cookingTime;
        public IngredientData CookedVersion => cookedVersion;
        public bool RequiresCutting => requiresCutting;
        public float CuttingTime => cuttingTime;
        public IngredientData CutVersion => cutVersion;
    }
}