using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatSpawner : MonoBehaviour
{
    [Header("Настройки времени спавна")]
    [Tooltip("Минимальное время ожидания перед появлением монстра (в секундах)")]
    [SerializeField] private float minSpawnDelay = 5f;

    [Tooltip("Максимальное время ожидания перед появлением монстра (в секундах)")]
    [SerializeField] private float maxSpawnDelay = 15f;

    [Header("Ссылки на внешние системы")]
    [Tooltip("Менеджер визуальных предупреждений (мигание UI)")]
    [SerializeField] private VisualWarningManager warningManager;

    // Список (хэш-таблица) для хранения сторон, где сейчас уже ЕСТЬ угроза
    private HashSet<KitchenSide> activeThreatSides = new HashSet<KitchenSide>();

    // Флаг, управляющий работой спавнера (можно отключить при паузе или проигрыше)
    private bool isSpawningActive = true;

    private void Start()
    {
        // Автоматически находим компонент на этом же объекте, если забыли привязать в Инспекторе
        if (warningManager == null)
        {
            warningManager = GetComponent<VisualWarningManager>();
        }

        // Как только игра запускается, мы включаем бесконечный таймер спавна
        StartCoroutine(SpawnTimerRoutine());
    }

    /// <summary>
    /// Корутина-таймер, которая работает параллельно игре и отсчитывает время
    /// </summary>
    private IEnumerator SpawnTimerRoutine()
    {
        while (isSpawningActive)
        {
            // Выбираем случайное число секунд в заданном нами диапазоне
            float nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);

            // Ждем это количество секунд перед тем, как пойти дальше по коду
            yield return new WaitForSeconds(nextSpawnTime);

            // Время прошло — вызываем метод выбора случайной угрозы
            SpawnRandomThreat();
        }
    }

    /// <summary>
    /// Метод выбирает случайную сторону кухни
    /// </summary>
    public void SpawnRandomThreat()
    {
        // Получаем массив всех доступных сторон из нашего Enum (исправлено на System.Enum)
        System.Array sides = System.Enum.GetValues(typeof(KitchenSide));

        // Выбираем случайный индекс из этого массива
        int randomIndex = Random.Range(0, sides.Length);
        KitchenSide randomSide = (KitchenSide)sides.GetValue(randomIndex);

        // Проверяем: если на этой стороне монстра еще НЕТ, то активируем его
        if (!activeThreatSides.Contains(randomSide))
        {
            ActivateThreat(randomSide);
        }
        else
        {
            // Если на этой стороне монстр уже есть, таймер просто уйдет на следующий круг
            Debug.Log($"Спавнер выбрал сторону {randomSide}, но там уже есть угроза.");
        }
    }

    /// <summary>
    /// Метод активации угрозы на конкретной стороне
    /// </summary>
    public void ActivateThreat(KitchenSide side)
    {
        // Добавляем сторону в список активных угроз
        activeThreatSides.Add(side);

        // Переводим название стороны в верхний регистр (LEFT, RIGHT и т.д.), как прописано в ТЗ
        string sideNameUpper = side.ToString().ToUpper();

        // Выводим требуемый текст в консоль Unity
        Debug.Log($"Monster spawned on {sideNameUpper} side");

        // Передаем конкретную сторону в менеджер предупреждений
        if (warningManager != null)
        {
            warningManager.ShowWarning(side);
        }
    }

    /// <summary>
    /// Публичный метод для очистки угрозы, когда игрок её ликвидировал
    /// </summary>
    public void ClearThreat(KitchenSide side)
    {
        if (activeThreatSides.Contains(side))
        {
            activeThreatSides.Remove(side);
            Debug.Log($"Threat cleared on {side.ToString().ToUpper()} side");

            // Если активных угроз больше не осталось, выключаем мигание предупреждения
            if (activeThreatSides.Count == 0 && warningManager != null)
            {
                warningManager.HideWarning();
            }
        }
    }
}