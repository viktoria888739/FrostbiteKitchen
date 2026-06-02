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
        // При запуске игры всегда показываем главное меню и прячем настройки
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenScene(int sceneIndex)
    {
        Debug.Log($"[SceneLoader] Загрузка сцены с индексом: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);

        if (GameStateMachine.Instance != null)
        {
            if (sceneIndex == 0)
            {
                GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.MainMenu);
            }
            else
            {
                GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Gameplay);
            }
        }
    }

    public void RestartCurrentScene()
    {
        Debug.Log("[SceneLoader] Перезапуск текущего уровня...");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Gameplay);
        }
    }

    public void OpenSettings()
    {
        // Включаем настройки, выключаем главное меню
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void CloseSetting()
    {
        // Выключаем настройки, возвращаем главное меню
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}