using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        Debug.Log("UIPauseManager: Awake");
    }

    private void OnEnable()
    {
        GameStateMachine.OnPauseEntered += ShowPausePanel;
        GameStateMachine.OnGameResumed += HidePausePanel;
    }

    private void OnDisable()
    {
        GameStateMachine.OnPauseEntered -= ShowPausePanel;
        GameStateMachine.OnGameResumed -= HidePausePanel;
    }

    private void Start()
    {
        Debug.Log("UIPauseManager: Start");
        HidePausePanel();
    }

    private void ShowPausePanel()
    {
        Debug.Log("ShowPausePanel вызван");
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("pausePanel = null!");
        }
    }
    private void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void OnResumeClicked()
    {
        GameStateMachine.Instance?.TogglePause();
    }

    public void OnExitClicked()
    {
        Debug.Log("Выход в главное меню");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}