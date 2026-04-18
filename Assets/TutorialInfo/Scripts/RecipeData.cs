using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Frostbite/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName; // Техническое имя (например, soup_01)
    public string displayName; // Имя для Василисы и игрока (например, "Уха по-антарктически")
    public List<IngridientData> requiredIngredients;
    public float timeLimit = 30f;
}
