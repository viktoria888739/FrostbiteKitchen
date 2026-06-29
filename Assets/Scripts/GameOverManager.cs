using UnityEngine;
using UnityEngine.SceneManagement;

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
            loader.OpenScene(SceneLoader.MainMenuSceneIndex);
        else
            SceneManager.LoadScene(SceneLoader.MainMenuSceneIndex);
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
