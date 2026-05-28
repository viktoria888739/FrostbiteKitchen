using System;
using UnityEngine;

namespace FrostbiteKitchen.Core
{
    public class SessionTimer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Длительность сессии в секундах (по ТЗ 180 секунд = 3 минуты)")]
        [SerializeField] private float sessionDuration = 180f; //

        private float _timeRemaining;
        private bool _isActive = false;

        // Событие для Василисы (UI), чтобы обновлять полосу времени или текст
        public static event Action<float> OnTimerUpdated;
        public static event Action OnTimerExpired;

        private void Start()
        {
            _timeRemaining = sessionDuration;
            
            // Подписываемся на состояние геймплея твоей машины
            GameStateMachine.OnStateChanged += HandleStateChanged; //
        }

        private void Update()
        {
            if (!_isActive) return;

            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                OnTimerUpdated?.Invoke(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _isActive = false;
                OnTimerExpired?.Invoke();
                HandleSessionEnd();
            }
        }

        private void HandleStateChanged(GameStateMachine.GameState newState) //
        {
            // Таймер идет только во время активного геймплея
            _isActive = (newState == GameStateMachine.GameState.Gameplay); //
        }

        private void HandleSessionEnd()
        {
            Debug.Log("[SessionTimer] Время демо-сессии вышло!");
            
            // Переключаем твой конечный автомат в состояние результатов
            if (GameStateMachine.Instance != null) //
            {
                GameStateMachine.Instance.ChangeState(GameStateMachine.GameState.Results); //
            }
        }

        private void OnDestroy()
        {
            GameStateMachine.OnStateChanged -= HandleStateChanged; //
        }

        // Публичный метод, если UI захочет узнать точный остаток времени в секундах
        public float GetTimeRemaining() => _timeRemaining;
    }
}