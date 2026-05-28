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
        Debug.Log("[GameStateMachine] Awake, DontDestroyOnLoad");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[GameStateMachine] OnEnable, подписка на загрузку сцен");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[GameStateMachine] OnDisable, отписка от загрузки сцен");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameStateMachine] Загружена сцена: {scene.name}");
        if (scene.name == "Gameplay") // Замените на точное имя вашей игровой сцены, если отличается
        {
            if (currentState != GameState.Gameplay)
            {
                Debug.Log("[GameStateMachine] Обнаружена загрузка Gameplay, переключаю состояние в Gameplay");
                ChangeState(GameState.Gameplay);
            }
        }
        else if (scene.name == "MainMenu")
        {
            if (currentState != GameState.MainMenu)
            {
                Debug.Log("[GameStateMachine] Обнаружена загрузка MainMenu, переключаю состояние в MainMenu");
                ChangeState(GameState.MainMenu);
            }
            Time.timeScale = 1f; // Сброс времени на всякий случай
        }
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log($"[GameStateMachine] ChangeState: {currentState} -> {newState}");
        if (currentState == newState)
        {
            Debug.Log("[GameStateMachine] Состояние не изменилось (уже такое)");
            return;
        }

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameStateMachine] Состояние изменено на {newState}");
    }

    private void EnterState(GameState state)
    {
        Debug.Log($"[GameStateMachine] EnterState: {state}");
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
            case GameState.MainMenu:
                Time.timeScale = 1;
                break;
            case GameState.Results:
                Time.timeScale = 1;
                break;
        }
    }

    private void ExitState(GameState state)
    {
        Debug.Log($"[GameStateMachine] ExitState: {state}");
        // Можно добавить логику выхода, если нужно
    }

    public void TogglePause()
    {
        Debug.Log($"[GameStateMachine] TogglePause, текущее состояние: {currentState}");
        if (currentState == GameState.Gameplay)
        {
            ChangeState(GameState.Pause);
        }
        else if (currentState == GameState.Pause)
        {
            ChangeState(GameState.Gameplay);
        }
        else
        {
            Debug.LogWarning($"[GameStateMachine] Нельзя переключить паузу из состояния {currentState}");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[GameStateMachine] Escape нажата");
            if (currentState == GameState.Gameplay || currentState == GameState.Pause)
                TogglePause();
        }
    }
}