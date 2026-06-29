using System.Collections;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }
    public static event System.Action OnPauseEntered;
    public static event System.Action OnGameResumed;
    public static event System.Action<GameState> OnStateChanged;

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Pause,
        Screamer,
        Results
    }

    [SerializeField] private GameState currentState = GameState.MainMenu;
    [SerializeField] private string screamerScreenObjectName = "ScreamerScreenUI";
    [SerializeField] private float screamerMinDuration = 3f;
    [SerializeField] private float screamerMaxDuration = 5f;

    private Coroutine screamerRoutine;

    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (currentState == GameState.Gameplay)
        {
            SessionStatistics.Instance?.StartSession();
            SessionResultEvaluator.Instance?.ResetStatistics();
            SessionOrderTracker.Instance?.ResetSession();
            GameAudioManager.Instance?.PlayKitchenAmbient();
        }
        else if (currentState == GameState.MainMenu)
        {
            GameAudioManager.Instance?.PlayMainMenuMusic();
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        GameState previousState = currentState;
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState, previousState);
        OnStateChanged?.Invoke(newState);
    }

    private void EnterState(GameState state, GameState previousState)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                SessionStatistics.Instance?.ResetStatistics();
                SessionResultEvaluator.Instance?.ResetStatistics();
                SessionOrderTracker.Instance?.ResetSession();
                GameOverManager.Instance?.PrepareForNewSession();
                HideResultsScreen();
                HideScreamerScreen();
                GameAudioManager.Instance?.PlayMainMenuMusic();
                break;

            case GameState.Gameplay:
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
                HideResultsScreen();
                HideScreamerScreen();
                GameOverManager.Instance?.PrepareForNewSession();
                ViewRotationBlocker.Reset();
                GameAudioManager.Instance?.PlayKitchenAmbient();

                if (previousState != GameState.Pause)
                {
                    SessionStatistics.Instance?.StartSession();
                    SessionResultEvaluator.Instance?.ResetStatistics();
                    SessionOrderTracker.Instance?.ResetSession();
                }
                break;

            case GameState.Pause:
                Time.timeScale = 0f;
                OnPauseEntered?.Invoke();
                break;

            case GameState.Screamer:
                Time.timeScale = 0f;
                GameAudioManager.Instance?.StopAllLoops();
                OrderManager.Instance?.StopManager();
                HideResultsScreen();
                ShowScreamerScreen();
                break;

            case GameState.Results:
                Time.timeScale = 0f;
                HideScreamerScreen();
                GameAudioManager.Instance?.StopAllLoops();
                SessionResultEvaluator.Instance?.EvaluateSessionResult();
                EndSessionAndShowFinalScreen();
                break;
        }
    }

    private void ExitState(GameState state)
    {
        if (state == GameState.Results)
            HideResultsScreen();

        if (state == GameState.Screamer)
            HideScreamerScreen();
    }

    private static void HideResultsScreen()
    {
        var gameOverDisplay = Object.FindFirstObjectByType<GameOverDisplay>(FindObjectsInactive.Include);
        if (gameOverDisplay != null)
            gameOverDisplay.gameObject.SetActive(false);
    }

    private void HideScreamerScreen()
    {
        if (screamerRoutine != null)
        {
            StopCoroutine(screamerRoutine);
            screamerRoutine = null;
        }

        GameObject screamer = FindScreamerObject();
        if (screamer != null)
            screamer.SetActive(false);
    }

    private void ShowScreamerScreen()
    {
        if (screamerRoutine != null)
            StopCoroutine(screamerRoutine);

        screamerRoutine = StartCoroutine(PlayScreamerSequence());
    }

    private GameObject FindScreamerObject()
    {
        if (string.IsNullOrEmpty(screamerScreenObjectName))
            return null;

        Transform[] transforms = Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (Transform transform in transforms)
        {
            GameObject candidate = transform.gameObject;
            if (candidate.name != screamerScreenObjectName)
                continue;

            if (!candidate.scene.IsValid())
                continue;

            return candidate;
        }

        return null;
    }

    private IEnumerator PlayScreamerSequence()
    {
        GameObject screamer = FindScreamerObject();

        if (screamer == null)
        {
            Debug.LogWarning("[GameStateMachine] ScreamerScreenUI не найден — переход сразу к Results.");
            screamerRoutine = null;
            ChangeState(GameState.Results);
            yield break;
        }

        screamer.SetActive(true);
        GameAudioManager.Instance?.PlayJumpscare();
        Debug.Log("[GameStateMachine] Показан экран скримера.");

        float duration = Mathf.Max(0f, UnityEngine.Random.Range(screamerMinDuration, screamerMaxDuration));
        yield return new WaitForSecondsRealtime(duration);

        screamer.SetActive(false);
        screamerRoutine = null;

        if (currentState == GameState.Screamer)
            ChangeState(GameState.Results);
    }

    private void EndSessionAndShowFinalScreen()
    {
        if (SessionStatistics.Instance == null)
            return;

        SessionStatistics.Instance.EndSession();

        var gameOverDisplay = Object.FindFirstObjectByType<GameOverDisplay>(FindObjectsInactive.Include);

        if (gameOverDisplay != null)
            gameOverDisplay.gameObject.SetActive(true);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Gameplay)
            ChangeState(GameState.Pause);
        else if (currentState == GameState.Pause)
            ChangeState(GameState.Gameplay);
    }
}
