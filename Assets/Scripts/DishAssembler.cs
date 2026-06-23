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

    public static System.Action<List<IngredientData>> OnPlateUpdated; 
    public static System.Action OnPlateCleared; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Гарантируем инициализацию списков
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

    /// <summary>
    /// Автоматическая заморозка/разморозка при смене состояний (особенно важно при угрозах)
    /// </summary>
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

    /// <summary>
    /// Замораживает текущее состояние сборки (вызывать при уходе на отражение угрозы)
    /// </summary>
    public void FreezeCurrentState()
    {
        if (isFrozen) return;

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
        Debug.Log($"[DishAssembler] Состояние ЗАМОРОЖЕНО. Сохранено ингредиентов: {frozenIngredientsBuffer.Count}");
    }

    /// <summary>
    /// Восстанавливает состояние без потери прогресса (при возвращении игрока)
    /// </summary>
    public void ResumeCurrentState()
    {
        if (!isFrozen) return;

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
        Debug.Log($"[DishAssembler] Состояние ВОССТАНОВЛЕНО. На тарелке: {ingredientsOnPlate.Count} ингредиентов");

        OnPlateUpdated?.Invoke(ingredientsOnPlate);
    }

    public void AddIngredient(IngredientData newIngredient) 
    { 
        if (newIngredient == null || isFrozen) 
        {
            Debug.LogWarning("[DishAssembler] Нельзя добавить ингредиент — сборка заморожена или null");
            return; 
        }
        
        ingredientsOnPlate ??= new List<IngredientData>();
        ingredientsOnPlate.Add(newIngredient); 
        
        Debug.Log($"[DishAssembler] Добавлен: {newIngredient.displayName}. Всего: {ingredientsOnPlate.Count}");
        OnPlateUpdated?.Invoke(ingredientsOnPlate); 
    } 

    public void ClearPlate() 
    { 
        ingredientsOnPlate?.Clear(); 
        frozenIngredientsBuffer?.Clear();
        isFrozen = false;

        Debug.Log("[DishAssembler] Тарелка полностью очищена.");
        OnPlateCleared?.Invoke(); 
    } 

    /// <summary>
    /// Публичный метод для OrderCompleteBtn и других систем
    /// </summary>
    public bool ValidateRecipe(RecipeData targetRecipe)
    {
        if (targetRecipe == null || targetRecipe.requiredIngredients == null || ingredientsOnPlate == null)
            return false;

        // Подсчёт требуемых ингредиентов
        Dictionary<IngredientData, int> required = new();
        foreach (var req in targetRecipe.requiredIngredients)
        {
            if (req.ingredient != null)
            {
                if (required.ContainsKey(req.ingredient))
                    required[req.ingredient] += req.count;
                else
                    required[req.ingredient] = req.count;
            }
        }

        // Подсчёт текущих
        Dictionary<IngredientData, int> current = new();
        foreach (var ing in ingredientsOnPlate)
        {
            if (ing != null)
            {
                if (current.ContainsKey(ing))
                    current[ing]++;
                else
                    current[ing] = 1;
            }
        }

        if (required.Count != current.Count) return false;

        foreach (var pair in required)
        {
            if (!current.TryGetValue(pair.Key, out int count) || count != pair.Value)
                return false;
        }

        return true;
    }

    // Для отладки в инспекторе
    [ContextMenu("Force Freeze State")]
    private void DebugFreeze() => FreezeCurrentState();

    [ContextMenu("Force Resume State")]
    private void DebugResume() => ResumeCurrentState();

    [ContextMenu("Clear Plate")]
    private void DebugClear() => ClearPlate();
}