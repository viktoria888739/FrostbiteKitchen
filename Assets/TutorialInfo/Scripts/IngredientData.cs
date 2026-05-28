using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Frostbite/Ingredient")]
public class IngredientData : ScriptableObject
{
    public string ingredientName; // �������� ��� ����
    public string displayName;    // �������� ��� ������ (�� �������)
    public Sprite icon;           // ��������, ������� �������� ������
    public Color debugColor = Color.white; // ����, ���� �������� ��� ���
}
