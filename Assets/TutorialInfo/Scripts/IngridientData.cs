using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Frostbite/Ingredient")]
public class IngridientData : ScriptableObject
{
    public string ingredientName; // Название для кода
    public string displayName;    // Название для игрока (на русском)
    public Sprite icon;           // Картинка, которую нарисует Эллина
    public Color debugColor = Color.white; // Цвет, если картинки еще нет
}
