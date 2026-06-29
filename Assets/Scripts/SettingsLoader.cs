using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSettingsData
{
    [Tooltip("Минимальная пауза между заказами в секундах")]
    public float minTimeBetweenOrdersSeconds = 3f;

    [Tooltip("Максимальная пауза между заказами в секундах")]
    public float maxTimeBetweenOrdersSeconds = 5f;

    [Tooltip("Процент успешно выполненных заказов для победы в демо")]
    public float winRequiredOrderPercentage = 50f;
}

public class SettingsLoader : MonoBehaviour
{
    public static SettingsLoader Instance { get; private set; }

    [Header("Config Settings")]
    [SerializeField] private string configFileName = "game_settings.json";

    public GameSettingsData CurrentSettings { get; private set; }

    private string SavePath => Path.Combine(Application.persistentDataPath, configFileName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                CurrentSettings = JsonUtility.FromJson<GameSettingsData>(json);
                Debug.Log($"[SettingsLoader] Настройки успешно загружены из: {SavePath}");
            }
            else
            {
                Debug.Log("[SettingsLoader] Файл настроек не найден. Создаю стандартный конфиг...");
                CreateDefaultSettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SettingsLoader] Ошибка при загрузке JSON: {e.Message}. Откат на стандартные настройки.");
            CreateDefaultSettings();
        }
    }

    private void CreateDefaultSettings()
    {
        CurrentSettings = new GameSettingsData();
        string json = JsonUtility.ToJson(CurrentSettings, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SettingsLoader] Стандартный конфиг сохранен по пути: {SavePath}");
    }
}