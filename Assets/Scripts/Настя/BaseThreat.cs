using System;
using UnityEngine;

public class BaseThreat : MonoBehaviour
{
    [Header("Основные параметры угрозы")]

    [Tooltip("Время действия угрозы в секундах до того, как наступит проигрыш/скример")]
    [SerializeField] protected float duration = 10f;

    [Tooltip("Скорость роста шкалы опасности (например, единиц в секунду)")]
    [SerializeField] protected float intensity = 1f;

    [Tooltip("Сторона кухни, на которой возникла эта угроза")]
    [SerializeField] protected KitchenSide location;

    // Свойства (Properties) для доступа к переменным из других скриптов (например, из спавнера)
    public float Duration => duration;
    public float Intensity => intensity;
    public KitchenSide Location => location;

    // Событие, которое сработает, когда время выйдет (игрок не успел устранить угрозу)
    public event Action<BaseThreat> OnThreatTimeout;

    // Внутренние переменные для отслеживания состояния
    protected float currentTimer = 0f;
    protected bool isActive = false;

    /// <summary>
    /// Метод инициализации/запуска угрозы
    /// </summary>
    public virtual void Initialize(KitchenSide spawnLocation)
    {
        location = spawnLocation;
        currentTimer = 0f;
        isActive = true;
    }

    protected virtual void Update()
    {
        if (!isActive) return;

        // Каждый кадр увеличиваем таймер на прошедшее время
        currentTimer += Time.deltaTime;

        // Здесь растет условная шкала опасности в зависимости от intensity
        // К примеру, можно накапливать общую опасность в каком-нибудь GameManager

        // Проверяем, не вышло ли время действия угрозы
        if (currentTimer >= duration)
        {
            TimeoutTriggered();
        }
    }

    /// <summary>
    /// Вызывается, когда время на устранение угрозы вышло
    /// </summary>
    protected virtual void TimeoutTriggered()
    {
        isActive = false;
        Debug.LogError($"[КРИТИЧЕСКАЯ УГРОЗА] Время вышло! Монстр на стороне {location} атаковал!");

        // Оповещаем другие системы (например, спавнер или менеджер игры)
        OnThreatTimeout?.Invoke(this);
    }

    /// <summary>
    /// Метод для кастомной логики нейтрализации угрозы игроком
    /// </summary>
    public virtual void Neutralize()
    {
        isActive = false;
        Debug.Log($"Угроза на стороне {location} успешно нейтрализована игроком.");
        Destroy(gameObject); // Уничтожаем объект угрозы на сцене
    }
}