using System.Collections;
using UnityEngine;
using FrostbiteKitchen.Data;
using FrostbiteKitchen.Gameplay;

public class Stove : MonoBehaviour, IInteractable
{
    [Header("Состояние плиты")]
    [SerializeField] private IngredientData currentIngredientOnStove;
    [SerializeField] private bool isCooking = false;
    [SerializeField] private bool isBurned = false;

    [Header("Анимация")]
    [SerializeField] private Animator animator;

    [Header("Порча во время угрозы")]
    [Tooltip("Через сколько секунд блюдо начинает подгорать")]
    [SerializeField] private float criticalOvercookTime = 6f;

    [Tooltip("Через сколько секунд блюдо полностью сгорает")]
    [SerializeField] private float burnTime = 11f;

    private float overcookTimer = 0f;
    private bool isUnderThreat = false;
    private Coroutine cookingCoroutine;

    public event System.Action<float> OnProgressUpdated;
    public event System.Action OnDishBurned;
    public event System.Action OnCookingFinished;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameStateMachine.OnStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateMachine.OnStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (!isCooking && currentIngredientOnStove != null && isUnderThreat && !isBurned)
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
        if (currentIngredientOnStove == null) return;

        PlayerInventory.Instance.SetHeldItem(currentIngredientOnStove, 1);
        ResetStove();
    }

    public bool TryPlaceIngredient(IngredientData ingredient)
    {
        if (currentIngredientOnStove != null || isCooking || ingredient == null) return false;
        if (!ingredient.RequiresCooking) return false;

        currentIngredientOnStove = ingredient;
        isBurned = false;
        StartCookingProcess();
        return true;
    }

    private void StartCookingProcess()
    {
        if (cookingCoroutine != null)
            StopCoroutine(cookingCoroutine);

        isCooking = true;
        if (animator != null) animator.SetTrigger("StartCooking");

        cookingCoroutine = StartCoroutine(CookingCoroutine(currentIngredientOnStove));
    }

    private IEnumerator CookingCoroutine(IngredientData rawIngredient)
    {
        float elapsed = 0f;
        float targetTime = rawIngredient.CookingTime;

        while (elapsed < targetTime)
        {
            if (!isCooking || currentIngredientOnStove == null)
                yield break;

            if (GameStateMachine.Instance != null && GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
            {
                yield return null;
                continue; 
            }

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / targetTime);
            OnProgressUpdated?.Invoke(progress);
            yield return null;
        }

        if (rawIngredient.CookedVersion != null && isCooking)
        {
            currentIngredientOnStove = rawIngredient.CookedVersion;
            if (animator != null) animator.SetTrigger("CookingDone");
            OnCookingFinished?.Invoke();
        }

        isCooking = false;
        cookingCoroutine = null;
        OnProgressUpdated?.Invoke(1f);
    }

    private void HandleOvercooking()
    {
        overcookTimer += Time.deltaTime;

        if (overcookTimer >= criticalOvercookTime && overcookTimer < burnTime)
        {
            if (overcookTimer < criticalOvercookTime + 0.5f)
            {
                Debug.Log("<color=orange>[ПЛИТА] Блюдо начинает подгорать...</color>");
            }
        }

        if (overcookTimer >= burnTime)
        {
            BurnDish();
        }
    }

    private void BurnDish()
    {
        if (currentIngredientOnStove == null) return;

        isBurned = true;
        isCooking = false;
        cookingCoroutine = null;

        Debug.Log($"<color=red>[ПЛИТА] 🔥 БЛЮДО СГОРЕЛО: {currentIngredientOnStove.displayName}</color>");

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddFailedOrder(); 

        if (animator != null) animator.SetTrigger("Burned");
        OnDishBurned?.Invoke();
    }

    private void ResetStove()
    {
        if (cookingCoroutine != null)
        {
            StopCoroutine(cookingCoroutine);
            cookingCoroutine = null;
        }

        currentIngredientOnStove = null;
        isCooking = false;
        isBurned = false;
        overcookTimer = 0f;
        OnProgressUpdated?.Invoke(0f);
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        isUnderThreat = (newState != GameStateMachine.GameState.Gameplay);
        
        if (animator != null) 
            animator.SetBool("IsUnderThreat", isUnderThreat);

    }
}