using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [Header("Gameplay HUD")]
    [SerializeField] private Button hudPauseButton;
    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("Settings Panel Buttons")]
    [SerializeField] private Button closeSettingsButton;

    private void Awake()
    {
        if (hudPauseButton != null) hudPauseButton.onClick.AddListener(TogglePauseState);
        if (resumeButton != null) resumeButton.onClick.AddListener(TogglePauseState);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitPressed);

        // Подписываем новые кнопки
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
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
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (hudPauseButton != null) hudPauseButton.gameObject.SetActive(false);
    }

    private void HidePauseUI()
    {
        pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (hudPauseButton != null) hudPauseButton.gameObject.SetActive(true);
    }
    
    private void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        if (settingsPanel != null) 
        {
            settingsPanel.SetActive(false);
        }
        if (pauseMenuPanel != null) 
        {
            UIWindowFade fadeComponent = pauseMenuPanel.GetComponent<UIWindowFade>();
            if (fadeComponent != null)
            {
                fadeComponent.SkipFadeNextTime();
            }
            
            pauseMenuPanel.SetActive(true);
        }
    }
    private void OnExitPressed()
    {
        Time.timeScale = 1;

        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.MainMenu);
        }

        SceneManager.LoadScene("MainMenu");
    }
}