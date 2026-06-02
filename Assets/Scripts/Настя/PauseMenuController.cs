using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Обязательно для смены сцен

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel; // Окно паузы

    [Header("Gameplay HUD")]
    [SerializeField] private Button hudPauseButton; // Кнопка "||" на экране во время игры

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        // Кнопка "||" на экране
        if (hudPauseButton != null)
            hudPauseButton.onClick.AddListener(TogglePauseState);

        // Кнопки внутри меню паузы
        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePauseState);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitPressed);

        // В начале игры меню паузы должно быть скрыто
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    private void OnEnable()
    {
        GameStateMachine.OnPauseEntered += ShowPauseUI;
        GameStateMachine.OnGameResumed += HidePauseUI;
    }

    private void OnDisable()
    {
        GameStateMachine.OnPauseEntered -= ShowPauseUI;
        GameStateMachine.OnGameResumed -= HidePauseUI;
    }

    // Метод для переключения состояния через State Machine
    private void TogglePauseState()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.TogglePause();
        }
    }

    private void ShowPauseUI()
    {
        pauseMenuPanel.SetActive(true);
        if (hudPauseButton != null) hudPauseButton.gameObject.SetActive(false); // Скрываем кнопку паузы, когда меню открыто
    }

    private void HidePauseUI()
    {
        pauseMenuPanel.SetActive(false);
        if (hudPauseButton != null) hudPauseButton.gameObject.SetActive(true); // Показываем кнопку паузы обратно
    }

    private void OnExitPressed()
    {
        // 1. Важно! Возвращаем время в нормальный режим перед сменой сцены
        Time.timeScale = 1;

        // 2. Опционально: уведомляем стейт-машину, что мы уходим в меню
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.MainMenu);
        }

        // 3. Загружаем сцену главного меню
        // Убедись, что сцена в окне Build Settings называется именно "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }
}