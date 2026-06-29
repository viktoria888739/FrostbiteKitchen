using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Panels (Only for Main Menu)")]
    [Tooltip("Панель с главными 3 кнопками (Start, Settings, Exit)")]
    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("Панель настроек с кнопкой выхода из них")]
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenScene(int sceneIndex)
    {
        Debug.Log($"[SceneLoader] Загрузка сцены с индексом: {sceneIndex}");
        Time.timeScale = 1f;
        GameOverManager.Instance?.PrepareForNewSession();
        SceneManager.LoadScene(sceneIndex);

        if (GameStateMachine.Instance != null)
        {
            if (sceneIndex == 0)
                GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.MainMenu);
            else
                GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Gameplay);
        }
    }

    public void RestartCurrentScene()
    {
        Debug.Log("[SceneLoader] Перезапуск текущего уровня...");
        Time.timeScale = 1f;
        GameOverManager.Instance?.PrepareForNewSession();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        if (GameStateMachine.Instance != null)
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Gameplay);
    }

    public void ReturnToMainMenu()
    {
        OpenScene(0);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void CloseSetting()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("[SceneLoader] Выход из игры");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}