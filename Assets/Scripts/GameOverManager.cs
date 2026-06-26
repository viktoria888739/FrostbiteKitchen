using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI Экрана Game Over")]
    [SerializeField] private GameObject gameOverScreenUI;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (gameOverScreenUI != null)
            gameOverScreenUI.SetActive(false);
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        SessionOrderTracker.OnSessionCompleted += OnSessionFinished;
    }

    private void OnDisable()
    {
        SessionOrderTracker.OnSessionCompleted -= OnSessionFinished;
    }

    private void OnSessionFinished()
    {
        if (!isGameOver)
            TriggerGameOver("Смена завершена! Все клиенты обслужены.");
    }

    public void TriggerGameOver(string reason = "Game Over")
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        Debug.Log($"<color=red>[GAME OVER] Причина: {reason}</color>");

        ShowGameOverScreen(reason);
    }

    private void ShowGameOverScreen(string reason)
    {
        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(true);
        }

        if (SessionStatistics.Instance != null)
        {
            Debug.Log($"=== GAME OVER ===\n" +
                      $"✅ Выполнено заказов: {SessionStatistics.Instance.completedOrders}\n" +
                      $"❌ Провалено заказов: {SessionStatistics.Instance.failedOrders}\n" +
                      $"⏱️ Общее время сессии: {SessionStatistics.Instance.sessionTime:F1} сек.");
        }
        else
        {
            Debug.LogWarning("[GameOverManager] SessionStatistics не найден!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
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