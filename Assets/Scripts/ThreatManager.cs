using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThreatManager : MonoBehaviour
{
    public static ThreatManager Instance { get; private set; }

    [Header("Настройки угроз")]
    [SerializeField] private float minTimeBetweenThreats = 25f;
    [SerializeField] private float maxTimeBetweenThreats = 45f;
    [SerializeField] private float threatDuration = 15f;

    [Header("Стороны кухни")]
    [SerializeField] private List<ThreatSpawner2> spawners = new List<ThreatSpawner2>();

    private bool isThreatActive = false;
    private Coroutine activeThreatCoroutine;
    private Coroutine threatSpawnCoroutine;

    public static System.Action OnThreatStarted;
    public static System.Action OnThreatCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    private void Start() => StartThreatSpawning();

    private void StartThreatSpawning()
    {
        if (threatSpawnCoroutine != null) StopCoroutine(threatSpawnCoroutine);
        threatSpawnCoroutine = StartCoroutine(ThreatSpawnRoutine());
    }

    private IEnumerator ThreatSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenThreats, maxTimeBetweenThreats));

            if (!isThreatActive && IsGameplayActive())
                StartRandomThreat();
        }
    }

    private bool IsGameplayActive()
    {
        return GameStateMachine.Instance != null && 
               GameStateMachine.Instance.CurrentState == GameStateMachine.GameState.Gameplay;
    }

    public void StartRandomThreat()
    {
        if (isThreatActive || spawners.Count == 0) return;

        ThreatSpawner2 spawner = spawners[Random.Range(0, spawners.Count)];
        isThreatActive = true;

        Debug.Log($"<color=red>[THREAT] 🚨 УГРОЗА со стороны: {spawner.sideName}</color>");

        spawner.ActivateThreat();
        OnThreatStarted?.Invoke();

        if (DishAssembler.Instance != null) DishAssembler.Instance.FreezeCurrentState();
        SetAllStovesThreatState(true);

        if (activeThreatCoroutine != null) StopCoroutine(activeThreatCoroutine);
        activeThreatCoroutine = StartCoroutine(ThreatDurationCoroutine(spawner));
    }

    private IEnumerator ThreatDurationCoroutine(ThreatSpawner2 spawner)
    {
        yield return new WaitForSeconds(threatDuration);
        if (isThreatActive)
            ClearCurrentThreat(spawner);
    }

    public void ClearCurrentThreat(ThreatSpawner2 spawner = null)
    {
        if (!isThreatActive) return;

        isThreatActive = false;
        if (spawner != null) spawner.DeactivateThreat();

        if (SessionStatistics.Instance != null)
            SessionStatistics.Instance.AddDefendedThreat();

        Debug.Log("<color=green>[THREAT] ✅ Угроза отражена</color>");

        OnThreatCleared?.Invoke();

        if (DishAssembler.Instance != null) DishAssembler.Instance.ResumeCurrentState();
        SetAllStovesThreatState(false);

        if (activeThreatCoroutine != null)
        {
            StopCoroutine(activeThreatCoroutine);
            activeThreatCoroutine = null;
        }
    }

    private void SetAllStovesThreatState(bool underThreat)
    {
        Stove[] stoves = Object.FindObjectsByType<Stove>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var stove in stoves)
        {
            if (stove != null) stove.SetThreatState(underThreat);
        }
    }

    public void PlayerDefendedThreat(ThreatSpawner2 spawner)
    {
        if (spawner == null) return;
        ClearCurrentThreat(spawner);
    }
}