using UnityEngine;
using System.Collections.Generic;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

public class DishAssembler : MonoBehaviour
{
    public static DishAssembler Instance { get; private set; }

    [Header("Current Plate Status")]
    [SerializeField] private List<IngredientData> ingredientsOnPlate = new();

    [Header("State Saving & Safety")]
    [SerializeField] private List<IngredientData> frozenIngredientsBuffer = new();
    [SerializeField] private bool isFrozen = false;

    [Header("Events")]
    public static System.Action<List<IngredientData>> OnDishChanged;
    public static System.Action OnPlateCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ingredientsOnPlate ??= new List<IngredientData>();
        frozenIngredientsBuffer ??= new List<IngredientData>();
    }

    private void OnEnable()
    {
        GameStateMachine.OnStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.OnStateChanged -= HandleGameStateChanged;
    }

    public int GetCurrentIngredientCount()
    {
        return ingredientsOnPlate?.Count ?? 0;
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        if (newState != GameStateMachine.GameState.Gameplay)
        {
            FreezeCurrentState();
        }
        else
        {
            ResumeCurrentState();
        }
    }

    public void FreezeCurrentState()
    {
        if (isFrozen) return;

        frozenIngredientsBuffer ??= new List<IngredientData>();
        frozenIngredientsBuffer.Clear();

        if (ingredientsOnPlate != null)
        {
            foreach (var ingredient in ingredientsOnPlate)
            {
                if (ingredient != null)
                    frozenIngredientsBuffer.Add(ingredient);
            }
        }

        isFrozen = true;
        Debug.Log($"[DishAssembler] Состояние ЗАМОРОЖЕНО. В буфер сохранено: {frozenIngredientsBuffer.Count} шт.");
    }

    public void ResumeCurrentState()
    {
        if (!isFrozen) return;

        ingredientsOnPlate ??= new List<IngredientData>();
        ingredientsOnPlate.Clear();

        if (frozenIngredientsBuffer != null)
        {
            foreach (var ingredient in frozenIngredientsBuffer)
            {
                if (ingredient != null)
                    ingredientsOnPlate.Add(ingredient);
            }
        }

        isFrozen = false;
        Debug.Log($"[DishAssembler] Состояние ВОССТАНОВЛЕНО. На тарелке снова: {ingredientsOnPlate.Count} шт.");

        OnDishChanged?.Invoke(ingredientsOnPlate);
    }

    public void AddIngredient(IngredientData newIngredient)
    {
        if (newIngredient == null)
        {
            Debug.LogWarning("[DishAssembler] Попытка добавить пустой ингредиент (null)!");
            return;
        }

        if (isFrozen)
        {
            frozenIngredientsBuffer ??= new List<IngredientData>();
            frozenIngredientsBuffer.Add(newIngredient);
            Debug.Log($"[DishAssembler] Добавлен в замороженный буфер: {newIngredient.displayName}");
            return;
        }

        ingredientsOnPlate ??= new List<IngredientData>();
        ingredientsOnPlate.Add(newIngredient);

        Debug.Log($"[DishAssembler] Ингредиент '{newIngredient.displayName}' добавлен. Всего: {ingredientsOnPlate.Count}");

        OnDishChanged?.Invoke(ingredientsOnPlate);
    }

    public void ClearPlate()
    {
        ingredientsOnPlate?.Clear();
        frozenIngredientsBuffer?.Clear();
        isFrozen = false;

        Debug.Log("[DishAssembler] Тарелка полностью очищена.");

        OnDishChanged?.Invoke(ingredientsOnPlate);
        OnPlateCleared?.Invoke();
    }
    public bool ValidateRecipe(RecipeData targetRecipe)
    {
        if (targetRecipe == null || targetRecipe.requiredIngredients == null)
            return false;

        var required = new Dictionary<IngredientData, int>();
        foreach (var req in targetRecipe.requiredIngredients)
        {
            if (req.ingredient != null)
                required[req.ingredient] = required.GetValueOrDefault(req.ingredient) + req.count;
        }

        var current = new Dictionary<IngredientData, int>();
        foreach (var ing in ingredientsOnPlate)
        {
            if (ing != null)
                current[ing] = current.GetValueOrDefault(ing) + 1;
        }

        if (required.Count != current.Count)
            return false;

        foreach (var pair in required)
        {
            if (!current.TryGetValue(pair.Key, out int count) || count != pair.Value)
                return false;
        }

        return true;
    }

    [ContextMenu("Force Freeze State")]
    private void DebugFreeze() => FreezeCurrentState();

    [ContextMenu("Force Resume State")]
    private void DebugResume() => ResumeCurrentState();

    [ContextMenu("Clear Plate")]
    private void DebugClear() => ClearPlate();
}