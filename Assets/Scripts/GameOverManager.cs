using UnityEngine;
using UnityEngine.SceneManagement;
using FrostbiteKitchen.Core;

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
        SessionTimer.OnTimerExpired += OnSessionTimeExpired;
    }

    private void OnDisable()
    {
        SessionTimer.OnTimerExpired -= OnSessionTimeExpired;
    }

    private void OnSessionTimeExpired()
    {
        if (!isGameOver)
            TriggerGameOver("Время сессии вышло");
    }

    public void TriggerGameOver(string reason = "Game Over")
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        Debug.Log($"<color=red>[GAME OVER] Причина: {reason}</color>");

        SessionData stats = SessionStatistics.Instance != null 
            ? SessionStatistics.Instance.GetSessionData() 
            : new SessionData();

        ShowGameOverScreen(stats, reason);
    }

    private void ShowGameOverScreen(SessionData stats, string reason)
    {
        if (gameOverScreenUI != null)
        {
            gameOverScreenUI.SetActive(true);
            // Здесь Василиса будет подтягивать данные
        }

        Debug.Log($"=== GAME OVER ===\n" +
                  $"✅ Выполнено блюд: {stats.successfulDishes}\n" +
                  $"❌ Испорчено: {stats.spoiledDishes}\n" +
                  $"🛡️ Отражено атак: {stats.threatsDefended}\n" +
                  $"⏱️ Время выживания: {stats.survivalTime:F1} сек.");
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