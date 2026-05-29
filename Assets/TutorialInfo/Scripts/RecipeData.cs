using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Frostbite/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName; // ����������� ��� (��������, soup_01)
    public string displayName; // ��� ��� �������� � ������ (��������, "��� ��-�������������")
    public List<IngredientData> requiredIngredients;
    public float timeLimit = 30f;
}
