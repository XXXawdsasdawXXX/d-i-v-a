using System;
using System.Collections;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Infrastructure.Services
{
    public class TimeObserver : IService, IGameInitListener, IGameTickListener, IProgressWriter
    {
        [Header("Static value")] private static readonly TimeSpan NightStart = new(22, 0, 0); // Начало ночи (20:00)
        private static readonly TimeSpan NightEnd = new(6, 0, 0); // Конец ночи (06:00)
        private RangedFloat _tickRangedTime;
        private float _tickTime;

        [Header("Dinamic value")] private float _currentTickCooldown;
        private DateTime _currentTime;
        private bool _isInit;
        private bool _isNight;
        private CoroutineRunner _coroutineRunner;

        public event Action TickEvent;
        public event Action<bool> InitTimeEvent;
        public event Action StartDayEvent;
        public event Action StartNightEvent;

        [Obsolete("Obsolete")]
        public void GameInit()
        {
            _tickRangedTime = Container.Instance.FindConfig<TimeConfig>().TickRangedTime;
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _tickTime = _tickRangedTime.GetRandomValue();

            Debugging.Instance.Log($"Current time {_currentTime}", Debugging.Type.Time);
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _coroutineRunner.StartRoutine(InitCurrentTime(playerProgress));
            Debugging.Instance.Log($"Load progress", Debugging.Type.Time);
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.GameExitTime = _currentTime;
            Debugging.Instance.Log($"Update progress", Debugging.Type.Time);
        }

        public void GameTick()
        {
            if (!_isInit)
            {
                return;
            }

            UpdateCurrentTime();
            UpdateTickTime();
        }

        private void UpdateCurrentTime()
        {
            _currentTime += TimeSpan.FromSeconds(Time.deltaTime);
        }

        private void UpdateTickTime()
        {
            _currentTickCooldown += Time.deltaTime;
            if (_currentTickCooldown >= _tickTime)
            {
                _tickTime = _tickRangedTime.GetRandomValue();
                _currentTickCooldown = 0;
                Debugging.Instance.Log($"Tick", Debugging.Type.Time);
                TickEvent?.Invoke();
                CheckTimeOfDay();
            }
        }

        private void CheckTimeOfDay()
        {
            if (IsNightTime() && !_isNight)
            {
                Debugging.Instance.Log($"Начало ночи", Debugging.Type.Time);
                _isNight = true;
                StartNightEvent?.Invoke();
            }
            else if (!IsNightTime() && _isNight)
            {
                Debugging.Instance.Log($"Начало дня", Debugging.Type.Time);
                _isNight = false;
                StartDayEvent?.Invoke();
            }
        }

        public bool IsNightTime()
        {
            TimeSpan timeOfDay = _currentTime.TimeOfDay;
            return (timeOfDay >= NightStart && timeOfDay < NightEnd);
        }


        [Obsolete("Obsolete")]
        private IEnumerator InitCurrentTime(PlayerProgressData playerProgressData)
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.google.com");
            if (myHttpWebRequest != null)
            {
                yield return myHttpWebRequest.Send();
                string netTime = myHttpWebRequest.GetResponseHeader("date");
                DateTime.TryParse(netTime, out _currentTime);
                Debugging.Instance.Log($"init google time", Debugging.Type.Time);
            }
            else
            {
                _currentTime = DateTime.UtcNow;
                Debugging.Instance.Log($"init standalone time", Debugging.Type.Time);
            }
            
            var lastVisit = playerProgressData.GameEnterTime; 
            playerProgressData.GameEnterTime = _currentTime;

            CheckTimeOfDay();
            
            _isInit = true;
            
            Debugging.Instance.Log($"End init: is first visit {!Extensions.IsEqualDay(lastVisit, _currentTime)}" +
                                   $"\ncurrent {_currentTime} saving {lastVisit}", Debugging.Type.Time);

            var isFirstVisit = !Extensions.IsEqualDay(lastVisit, _currentTime);
            if (isFirstVisit)
            {
                playerProgressData.CustomActions = new CustomActionsSavedData();
            }
            InitTimeEvent?.Invoke(isFirstVisit);
        }



        #region Editor

        public KeyValuePair<float, float> GetTimeState()
        {
            return new KeyValuePair<float, float>(_currentTickCooldown, _tickTime);
        }

        #endregion
    }
}