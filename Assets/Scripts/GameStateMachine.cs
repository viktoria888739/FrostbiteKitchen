using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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

    [Header("Current Status")]
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
        }
    }

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameStateMachine] Переход в: {newState}");
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();

                SessionStatistics.Instance?.StartSession();
                SessionResultEvaluator.Instance?.ResetStatistics();
                SessionOrderTracker.Instance?.ResetSession();
                break;

            case GameState.Pause:
                Time.timeScale = 0f;
                OnPauseEntered?.Invoke();
                break;

            case GameState.Results:
                SessionResultEvaluator.Instance?.EvaluateSessionResult();
                EndSessionAndShowFinalScreen();
                break;
        }
    }

    private void ExitState(GameState state) { }

    private void EndSessionAndShowFinalScreen()
    {
        if (SessionStatistics.Instance == null)
        {
            Debug.LogError("[GameStateMachine] SessionStatistics не найден!");
            return;
        }

        SessionStatistics.Instance.EndSession();
        var result = SessionResultEvaluator.Instance != null
            ? SessionResultEvaluator.Instance.CurrentResult
            : SessionResultEvaluator.SessionStatus.Fail;

        var gameOverDisplay = UnityEngine.Object.FindFirstObjectByType<GameOverDisplay>(FindObjectsInactive.Include);

        if (gameOverDisplay != null)
        {
            gameOverDisplay.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[GameStateMachine] GameOverDisplay не найден!");
        }
    }

    public void TogglePause()
    {
        if (currentState == GameState.Gameplay)
            ChangeState(GameState.Pause);
        else if (currentState == GameState.Pause)
            ChangeState(GameState.Gameplay);
    }
}