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

    private bool isThreatActive;
    private KitchenSide activeSide;
    private ThreatSpawner2 activeSpawner;
    private Coroutine activeThreatCoroutine;
    private Coroutine threatSpawnCoroutine;

    public bool IsThreatActive => isThreatActive;
    public KitchenSide ActiveSide => activeSide;

    public static System.Action<KitchenSide> OnThreatStarted;
    public static System.Action OnThreatCleared;
    public static System.Action OnThreatFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartThreatSpawning();
    }

    public bool IsActiveThreatOn(KitchenSide side)
    {
        return isThreatActive && activeSide == side;
    }

    private void StartThreatSpawning()
    {
        if (threatSpawnCoroutine != null)
            StopCoroutine(threatSpawnCoroutine);

        threatSpawnCoroutine = StartCoroutine(ThreatSpawnRoutine());
    }

    private void StopThreatSpawning()
    {
        if (threatSpawnCoroutine == null)
            return;

        StopCoroutine(threatSpawnCoroutine);
        threatSpawnCoroutine = null;
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
        if (isThreatActive || spawners.Count == 0)
            return;

        ThreatSpawner2 chosenSpawner = spawners[Random.Range(0, spawners.Count)];
        if (chosenSpawner == null)
            return;

        if (!System.Enum.TryParse(chosenSpawner.sideName, out KitchenSide sideEnum))
        {
            Debug.LogWarning($"[ThreatManager] Не удалось распознать сторону: {chosenSpawner.sideName}");
            sideEnum = KitchenSide.Front;
        }

        isThreatActive = true;
        activeSide = sideEnum;
        activeSpawner = chosenSpawner;

        Debug.Log($"<color=red>[THREAT] УГРОЗА со стороны: {activeSide}</color>");

        chosenSpawner.ActivateThreat();
        OnThreatStarted?.Invoke(activeSide);
        GameAudioManager.Instance?.PlayThreatSpawn(activeSide);

        if (activeThreatCoroutine != null)
            StopCoroutine(activeThreatCoroutine);

        activeThreatCoroutine = StartCoroutine(ThreatDurationCoroutine());
    }

    private IEnumerator ThreatDurationCoroutine()
    {
        yield return new WaitForSeconds(threatDuration);

        if (isThreatActive)
            HandleThreatExpired();
    }

    private void HandleThreatExpired()
    {
        if (!isThreatActive)
            return;

        if (GameStateMachine.Instance != null &&
            GameStateMachine.Instance.CurrentState != GameStateMachine.GameState.Gameplay)
        {
            CancelActiveThreatSilently();
            return;
        }

        ThreatSpawner2 spawner = activeSpawner;
        isThreatActive = false;
        activeSpawner = null;

        spawner?.DeactivateThreat();

        Debug.LogError("[THREAT] Угроза не отбита вовремя!");

        OnThreatFailed?.Invoke();
        SessionResultEvaluator.Instance?.HandleThreatMissed();
        StopThreatSpawning();

        if (activeThreatCoroutine != null)
        {
            StopCoroutine(activeThreatCoroutine);
            activeThreatCoroutine = null;
        }
    }

    public void PlayerDefendedThreat(ThreatSpawner2 spawner)
    {
        if (!isThreatActive || spawner == null || spawner != activeSpawner)
            return;

        ClearDefendedThreat();
    }

    public void PlayerDefendedThreat(KitchenSide defendedSide)
    {
        if (!isThreatActive || activeSide != defendedSide)
            return;

        ClearDefendedThreat();
    }

    private void ClearDefendedThreat()
    {
        if (!isThreatActive)
            return;

        ThreatSpawner2 spawner = activeSpawner;
        isThreatActive = false;
        activeSpawner = null;

        spawner?.DeactivateThreat();

        SessionStatistics.Instance?.AddDefendedThreat();
        Debug.Log("<color=green>[THREAT] Угроза отражена</color>");

        OnThreatCleared?.Invoke();

        if (activeThreatCoroutine != null)
        {
            StopCoroutine(activeThreatCoroutine);
            activeThreatCoroutine = null;
        }
    }

    public void CancelSessionThreats()
    {
        StopThreatSpawning();
        CancelActiveThreatSilently();
    }

    private void CancelActiveThreatSilently()
    {
        if (!isThreatActive)
            return;

        ThreatSpawner2 spawner = activeSpawner;
        isThreatActive = false;
        activeSpawner = null;

        spawner?.DeactivateThreat();
        OnThreatCleared?.Invoke();

        if (activeThreatCoroutine != null)
        {
            StopCoroutine(activeThreatCoroutine);
            activeThreatCoroutine = null;
        }
    }
}
