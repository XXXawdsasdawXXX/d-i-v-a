using System;
using System.Collections;
using System.Collections.Generic;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Services
{
    public class TimeObserver : IService, IGameInitListener, IGameTickListener
    {
        private static readonly TimeSpan NightStart = new(22, 0, 0); // Начало ночи (20:00)
        private static readonly TimeSpan NightEnd = new(6, 0, 0); // Конец ночи (06:00)

        private RangedFloat _tickRangedTime;
        private DateTime _currentTime;
        
        private float _currentTickCooldown;
        private  float _tickTime;

        private bool _isInit;
        private bool _isNight;

        public event Action TickEvent;
        public event Action InitTimeEvent;
        public event Action StartDayEvent;
        public event Action StartNightEvent;

        [Obsolete("Obsolete")]
        public void GameInit()
        {
            _tickRangedTime = Container.Instance.FindConfig<TimeConfig>().TickRangedTime;
            
            var coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            if (coroutineRunner == null)
            {
                Debugging.Instance.Log($"time observer can't find coroutine runner", Debugging.Type.Time);
                return;
            }

            _tickTime = _tickRangedTime.GetRandomValue();
            coroutineRunner.StartRoutine(InitCurrentTime());

            Debugging.Instance.Log($"Current time {_currentTime}", Debugging.Type.Time);
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
                StartNightEvent?.Invoke();
            }
            else if (!IsNightTime() && _isNight)
            {
                StartDayEvent?.Invoke();
            }
        }

        public bool IsNightTime()
        {
            TimeSpan timeOfDay = _currentTime.TimeOfDay;
            return (timeOfDay >= NightStart || timeOfDay < NightEnd);
        }


        [Obsolete("Obsolete")]
        private IEnumerator InitCurrentTime()
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.google.com");
            if (myHttpWebRequest == null)
            {
                _currentTime = DateTime.UtcNow;
                _isInit = true;
                InitTimeEvent?.Invoke();
                yield break;
            }

            yield return myHttpWebRequest.Send();
            string netTime = myHttpWebRequest.GetResponseHeader("date");
            Debugging.Instance.Log($"net time {netTime}", Debugging.Type.Time);
            DateTime.TryParse(netTime, out _currentTime);
            InitTimeEvent?.Invoke();
            _isInit = true;
            CheckTimeOfDay();
        }

        #region Editor

        public KeyValuePair<float, float> GetTimeState()
        {
            return new KeyValuePair<float, float>(_currentTickCooldown, _tickTime);
        }
        
        #endregion
        
    }
}