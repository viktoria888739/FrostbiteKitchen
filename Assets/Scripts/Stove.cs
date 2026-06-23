using System.Collections;
using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

public class Stove : MonoBehaviour, IInteractable
{
    [Header("Состояние плиты")]
    [SerializeField] private IngredientData currentIngredientOnStove;
    [SerializeField] private bool isCooking = false;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    [Header("Порча во время угрозы")]
    [Tooltip("Время (сек), после которого блюдо начинает портиться во время угрозы")]
    [SerializeField] private float criticalOvercookTime = 6f;
    
    [Tooltip("Время (сек), через которое блюдо полностью сгорает")]
    [SerializeField] private float burnTime = 11f;

    private float overcookTimer = 0f;
    private bool isUnderThreat = false;

    public event System.Action OnDishBurned;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isCooking && currentIngredientOnStove != null && isUnderThreat)
        {
            HandleOvercooking();
        }
    }

    public void SetThreatState(bool underThreat)
    {
        isUnderThreat = underThreat;
        if (animator != null)
            animator.SetBool("IsUnderThreat", underThreat);

        if (!underThreat) 
            overcookTimer = 0f;
    }

    public void Interact()
    {
        // Положить ингредиент
        if (!isCooking && currentIngredientOnStove == null && PlayerInventory.Instance.CurrentHeldItem != null)
        {
            if (PlayerInventory.Instance.CurrentAmount == 1)
            {
                IngredientData input = PlayerInventory.Instance.CurrentHeldItem;
                if (input.RequiresCooking)
                {
                    PlayerInventory.Instance.ClearInventory();
                    StartCooking(input);
                }
                else
                {
                    Debug.Log($"<color=red>[ПЛИТА]</color> {input.displayName} не требует жарки!");
                }
            }
            return;
        }

        // Забрать готовое блюдо
        if (!isCooking && currentIngredientOnStove != null && PlayerInventory.Instance.CurrentHeldItem == null)
        {
            PlayerInventory.Instance.SetHeldItem(currentIngredientOnStove, 1);
            ResetStove();
        }
    }

    private void StartCooking(IngredientData input)
    {
        currentIngredientOnStove = input;
        isCooking = true;

        if (animator != null) animator.SetTrigger("StartCooking");
        StartCoroutine(CookCoroutine(input));
    }

    private IEnumerator CookCoroutine(IngredientData input)
    {
        float targetTime = input.CookingTime;
        yield return new WaitForSeconds(targetTime);

        if (input.CookedVersion != null)
        {
            currentIngredientOnStove = input.CookedVersion;
            if (animator != null) animator.SetTrigger("CookingDone");
        }

        isCooking = false;
    }

    private void HandleOvercooking()
    {
        overcookTimer += Time.deltaTime;

        // Используем criticalOvercookTime — warning убран
        if (overcookTimer >= criticalOvercookTime && overcookTimer >= burnTime)
        {
            BurnDish();
        }
    }

    private void BurnDish()
    {
        if (currentIngredientOnStove == null) return;

        Debug.Log($"<color=red>[ПЛИТА] 🔥 БЛЮДО СГОРЕЛО: {currentIngredientOnStove.displayName}</color>");

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddSpoiledDish();

        if (animator != null) animator.SetTrigger("Burned");

        OnDishBurned?.Invoke();
        ResetStove();
    }

    private void ResetStove()
    {
        currentIngredientOnStove = null;
        overcookTimer = 0f;
        isCooking = false;

        if (animator != null) animator.SetTrigger("ResetStove");
    }
}