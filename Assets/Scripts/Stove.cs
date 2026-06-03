using System.Collections;
using UnityEngine;
using FrostbiteKitchen.Data;
using UnityEngine.EventSystems;
using FrostbiteKitchen.Gameplay;

public class Stove : MonoBehaviour, IInteractable
{
    [Header("Состояние плиты")]
    [SerializeField] private IngredientData currentIngredientOnStove;
    [SerializeField] private bool isCooking = false;
    [SerializeField] private float cookingProgress = 0f;

    // События для UI (полоска прогресса)
    public delegate void OnCookingProgressChanged(float current, float max);
    public event OnCookingProgressChanged OnProgressUpdated;

    public void Interact()
    {
        // ВАРИАНТ 1: Кладём ингредиент на пустую плиту
        if (!isCooking && currentIngredientOnStove == null && PlayerInventory.Instance.CurrentHeldItem != null)
        {
            if (PlayerInventory.Instance.CurrentAmount == 1)
            {
                IngredientData input = PlayerInventory.Instance.CurrentHeldItem;

                if (input.RequiresCooking)
                {
                    PlayerInventory.Instance.ClearInventory(); // Забираем у игрока
                    Process(input);
                }
                else
                {
                    Debug.Log($"<color=red>[ПЛИТА]</color> {input.displayName} не требует жарки!");
                }
            }
            return;
        }

        // ВАРИАНТ 2: Забираем готовый продукт
        if (!isCooking && currentIngredientOnStove != null && PlayerInventory.Instance.CurrentHeldItem == null)
        {
            Debug.Log($"<color=yellow>[ПЛИТА]</color> Игрок забрал: {currentIngredientOnStove.displayName}");

            PlayerInventory.Instance.SetHeldItem(currentIngredientOnStove, 1);
            currentIngredientOnStove = null;
        }
    }

    public void Process(IngredientData input)
    {
        currentIngredientOnStove = input;
        StartCoroutine(CookCoroutine(input));
    }

    private IEnumerator CookCoroutine(IngredientData input)
    {
        isCooking = true;
        cookingProgress = 0f;
        float targetTime = input.CookingTime;

        Debug.Log($"<color=yellow>[ПЛИТА]</color> Начата жарка: {input.displayName} ({targetTime} сек.)");

        while (cookingProgress < targetTime)
        {
            cookingProgress += Time.deltaTime;
            OnProgressUpdated?.Invoke(cookingProgress, targetTime);
            yield return null;
        }

        // Жарка завершена
        if (input.CookedVersion != null)
        {
            currentIngredientOnStove = input.CookedVersion;
            Debug.Log($"<color=green>[ПЛИТА]</color> Готово! Получено: {currentIngredientOnStove.displayName}");
        }
        else
        {
            Debug.LogError($"[ПЛИТА] У {input.displayName} не назначена CookedVersion!");
        }

        isCooking = false;
        OnProgressUpdated?.Invoke(0f, targetTime); // Сброс UI
    }
}