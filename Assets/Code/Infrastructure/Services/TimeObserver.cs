using System;
using System.Collections;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Infrastructure.Services
{
    public class TimeObserver : IService, IInitListener, IStartListener ,IUpdateListener, IProgressWriter
    {
        public event Action<bool> OnTimeInitialized;
        public event Action OnTicked;
        public event Action OnDayStarted;
        public event Action OnNightStarted;

        [Header("Static value")] 
        private static readonly TimeSpan NightStart = new(22, 0, 0);
        private static readonly TimeSpan NightEnd = new(6, 0, 0);
        private RangedFloat _tickRangedTime;
        private float _tickDuration;

        [Header("Dynamic value")] 
        private float _currentDeltaTime;
        private DateTime _currentTime;
        private bool _isInit;
        private bool _isNight;
        private int _tickCount;

        private CoroutineRunner _coroutineRunner;

        public void GameInitialize()
        {
            _tickRangedTime = Container.Instance.FindConfig<TimeConfig>().TickRangedTime;
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            
#if DEBUGGING
            Debugging.Log(this, $"[Init] Current time {_currentTime}.", Debugging.Type.Time);
#endif
        }

        public void GameStart()
        {
            _tickDuration = _tickRangedTime.GetRandomValue();
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
            _currentDeltaTime += Time.deltaTime;

            if (_currentDeltaTime >= _tickDuration)
            {
                _tickDuration = _tickRangedTime.GetRandomValue();

                _currentDeltaTime = 0;

                _tickCount++;
#if DEBUGGING
                Debugging.Log(this, $"[_updateTickTime] Tick #{_tickCount}.", Debugging.Type.Time);
#endif

                OnTicked?.Invoke();

                _checkTimeOfDay();
            }
        }

        private void _checkTimeOfDay()
        {
            bool isNightTime = IsNightTime();

            if (isNightTime && !_isNight)
            {
                _isNight = true;

                OnNightStarted?.Invoke();
#if DEBUGGING
                Debugging.Log(this, "[_checkTimeOfDay] Start night.", Debugging.Type.Time);
#endif
            }
            else if (!isNightTime && _isNight)
            {
                _isNight = false;

                OnDayStarted?.Invoke();
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

            OnTimeInitialized?.Invoke(isFirstVisit);
        }

        #region Editor

        public float GetCurrentTick()
        {
            return _currentDeltaTime;
        }

        public float GetTickDuration()
        {
            return _tickDuration;
        }

        #endregion
    }
}