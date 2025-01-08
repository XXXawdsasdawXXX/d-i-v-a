using System;
using System.Collections;
using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Infrastructure.Services
{
    public class TimeObserver : IService, IGameInitListener, IGameUpdateListener, IProgressWriter
    {
        public event Action TickEvent;
        public event Action<bool> InitTimeEvent;
        public event Action StartDayEvent;
        public event Action StartNightEvent;

        [Header("Static value")] 
        private static readonly TimeSpan NightStart = new(22, 0, 0);
        private static readonly TimeSpan NightEnd = new(6, 0, 0);
        private RangedFloat _tickRangedTime;
        private float _tickDuration;

        [Header("Dynamic value")] 
        private float _currentTick;
        private DateTime _currentTime;
        private bool _isInit;
        private bool _isNight;
        private CoroutineRunner _coroutineRunner;

        public void GameInit()
        {
            _tickRangedTime = Container.Instance.FindConfig<TimeConfig>().TickRangedTime;
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();

            _tickDuration = _tickRangedTime.GetRandomValue();
            
#if DEBUGGING
            Debugging.Log(this, $"[Init] Current time {_currentTime}.", Debugging.Type.Time);
#endif
        }

        public void GameUpdate()
        {
            if (!_isInit)
            {
                return;
            }

            _updateCurrentTime();

            _updateTickTime();
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _coroutineRunner.StartRoutine(_initCurrentTime(playerProgress));

#if DEBUGGING
            Debugging.Log(this, "[Load]", Debugging.Type.Time);
#endif
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.GameExitTime = _currentTime;

#if DEBUGGING
            Debugging.Log(this, "[Save]", Debugging.Type.Time);
#endif
        }

        public bool IsNightTime()
        {
            TimeSpan timeOfDay = _currentTime.TimeOfDay;

            if (NightStart < NightEnd)
            {
                return timeOfDay >= NightStart && timeOfDay < NightEnd;
            }

            return timeOfDay >= NightStart || timeOfDay < NightEnd;
        }

        private void _updateCurrentTime()
        {
            _currentTime += TimeSpan.FromSeconds(Time.deltaTime);
        }

        private void _updateTickTime()
        {
            _currentTick += Time.deltaTime;

            if (_currentTick >= _tickDuration)
            {
                _tickDuration = _tickRangedTime.GetRandomValue();

                _currentTick = 0;

#if DEBUGGING
                Debugging.Log(this, "Tick", Debugging.Type.Time);
#endif

                TickEvent?.Invoke();

                _checkTimeOfDay();
            }
        }

        private void _checkTimeOfDay()
        {
            bool isNightTime = IsNightTime();

            if (isNightTime && !_isNight)
            {
                _isNight = true;

                StartNightEvent?.Invoke();
#if DEBUGGING
                Debugging.Log(this, "[_checkTimeOfDay] Start night.", Debugging.Type.Time);
#endif
            }
            else if (!isNightTime && _isNight)
            {
                _isNight = false;

                StartDayEvent?.Invoke();
#if DEBUGGING
                Debugging.Log(this, "[_checkTimeOfDay] Start day.", Debugging.Type.Time);
#endif
            }
        }

        private IEnumerator _initCurrentTime(PlayerProgressData playerProgressData)
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("https://www.google.com");

            if (myHttpWebRequest != null)
            {
                yield return myHttpWebRequest.SendWebRequest();

                string netTime = myHttpWebRequest.GetResponseHeader("date");
#if DEBUGGING
                Debugging.Log(this, $"[_initCurrentTime] Init google time. Time = {netTime}", Debugging.Type.Time);
#endif
                if (!DateTime.TryParse(netTime, out _currentTime))
                {
                    _currentTime = DateTime.UtcNow;
#if DEBUGGING
                    Debugging.Log(this, $"[_initCurrentTime] Lose google time parsing. Time = {_currentTime}",
                        Debugging.Type.Time);
#endif
                }
            }
            else
            {
                _currentTime = DateTime.UtcNow;
#if DEBUGGING
                Debugging.Log(this, $"[_initCurrentTime] Init standalone time. Time = {_currentTime}",
                    Debugging.Type.Time);
#endif
            }

            DateTime lastVisit = playerProgressData.GameEnterTime;

            playerProgressData.GameEnterTime = _currentTime;

            _checkTimeOfDay();

            _isInit = true;

#if DEBUGGING
            Debugging.Log(this, $" [_initCurrentTime] End init.\n" +
                                $"Is first visit = {!Extensions.IsEqualDay(lastVisit, _currentTime)}\n" +
                                $"Current time =  {_currentTime}. Saving time = {lastVisit}",
                Debugging.Type.Time);
#endif
            
            bool isFirstVisit = !Extensions.IsEqualDay(lastVisit, _currentTime);

            if (isFirstVisit)
            {
                playerProgressData.CustomActions = new CustomActionsSavedData();
            }

            InitTimeEvent?.Invoke(isFirstVisit);
        }

        #region Editor

        public float GetCurrentTick()
        {
            return _currentTick;
        }

        public float GetTickDuration()
        {
            return _tickDuration;
        }

        #endregion
    }
}