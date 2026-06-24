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
    [Tooltip("Время (сек), после которого блюдо начинает портиться")]
    [SerializeField] private float criticalOvercookTime = 6f;
    
    [Tooltip("Время (сек), через которое блюдо полностью сгорает")]
    [SerializeField] private float burnTime = 11f;

    private float overcookTimer = 0f;
    private bool isUnderThreat = false;

    public event System.Action<float> OnProgressUpdated;
    public event System.Action OnDishBurned;

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
        
        string status = isBurned ? "сгоревшее" : "готовое";
        Debug.Log($"<color=yellow>[ПЛИТА]</color> Забрали {status} блюдо: {currentIngredientOnStove.displayName}");

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
        isCooking = true;
        if (animator != null) animator.SetTrigger("StartCooking");
        StartCoroutine(CookingCoroutine(currentIngredientOnStove));
    }

    private IEnumerator CookingCoroutine(IngredientData rawIngredient)
    {
        float elapsed = 0f;
        float targetTime = rawIngredient.CookingTime;

        while (elapsed < targetTime)
        {
            if (isUnderThreat)
            {
                yield return null;
                continue;
            }

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / targetTime);
            OnProgressUpdated?.Invoke(progress);
            yield return null;
        }

        if (rawIngredient.CookedVersion != null)
        {
            currentIngredientOnStove = rawIngredient.CookedVersion;
            if (animator != null) animator.SetTrigger("CookingDone");
        }

        isCooking = false;
        OnProgressUpdated?.Invoke(1f);
    }

    private void HandleOvercooking()
    {
        overcookTimer += Time.deltaTime;

        if (overcookTimer >= criticalOvercookTime && overcookTimer >= burnTime)
        {
            BurnDish();
        }
    }

    private void BurnDish()
    {
        if (currentIngredientOnStove == null) return;

        isBurned = true;
        isCooking = false;

        Debug.Log($"<color=red>[ПЛИТА] 🔥 БЛЮДО СГОРЕЛО: {currentIngredientOnStove.displayName}</color>");

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddSpoiledDish();

        if (animator != null) animator.SetTrigger("Burned");

        OnDishBurned?.Invoke();
    }

    private void ResetStove()
    {
        currentIngredientOnStove = null;
        isCooking = false;
        isBurned = false;
        overcookTimer = 0f;
        OnProgressUpdated?.Invoke(0f);
    }

    private void HandleGameStateChanged(GameStateMachine.GameState newState)
    {
        isUnderThreat = (newState != GameStateMachine.GameState.Gameplay);
    }
}