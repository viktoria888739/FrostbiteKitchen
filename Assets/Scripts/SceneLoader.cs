using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Panels (Only for Main Menu)")]
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
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
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSetting()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
