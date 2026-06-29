using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Legacy-обёртка для кнопок на экране результатов.
/// Вся логика показа и паузы — в GameStateMachine + GameOverDisplay.
/// </summary>
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

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

    public void PrepareForNewSession()
    {
        // Сброс внутреннего состояния после рестарта или выхода в меню.
    }

    public void RestartGame()
    {
        var loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader != null)
            loader.RestartCurrentScene();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        var loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader != null)
            loader.OpenScene(0);
        else
            SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
