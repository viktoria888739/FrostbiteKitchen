using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }
    public static event Action OnPauseEntered;
    public static event Action OnGameResumed;
    public static event Action<GameState> OnStateChanged;

    public enum GameState
    {
        MainMenu,
        Gameplay,
        Pause,
        Results
    }

    [Header("Current Status")]
    [SerializeField] private GameState currentState;
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

    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[Game Manager] Switched to: {newState}");
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Pause:
                Time.timeScale = 0;
                OnPauseEntered?.Invoke();
                break;
            case GameState.Gameplay:
                Time.timeScale = 1;
                OnGameResumed?.Invoke();
                break;
        }
    }

    private void ExitState(GameState state) { }

    public void TogglePause()
    {
        if (currentState == GameState.Gameplay) ChangeState(GameState.Pause);
        else if (currentState == GameState.Pause) ChangeState(GameState.Gameplay);
    }
}