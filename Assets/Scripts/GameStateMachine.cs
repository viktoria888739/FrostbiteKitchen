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
        Results
    }

    [SerializeField] private GameState currentState = GameState.MainMenu;
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
                GameAudioManager.Instance?.PlayMainMenuMusic();
                break;

            case GameState.Gameplay:
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
                HideResultsScreen();
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

            case GameState.Results:
                Time.timeScale = 0f;
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
    }

    private static void HideResultsScreen()
    {
        var gameOverDisplay = Object.FindFirstObjectByType<GameOverDisplay>(FindObjectsInactive.Include);
        if (gameOverDisplay != null)
            gameOverDisplay.gameObject.SetActive(false);
    }

    private void EndSessionAndShowFinalScreen()
    {
        if (SessionStatistics.Instance == null)
            return;

        SessionStatistics.Instance.EndSession();

        var gameOverDisplay = UnityEngine.Object.FindFirstObjectByType<GameOverDisplay>(FindObjectsInactive.Include);

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