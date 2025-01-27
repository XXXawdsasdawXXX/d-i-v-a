using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Services
{
    [Preserve]
    public class TimeObserver : IService, IInitListener, IStartListener, IUpdateListener, IProgressWriter
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
        private bool _isNight;
        private int _tickCount;

        public UniTask GameInitialize()
        {
            _tickRangedTime = Container.Instance.FindConfig<TimeConfig>().TickRangedTime;

            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            _tickDuration = _tickRangedTime.GetRandomValue();

            return UniTask.CompletedTask;
        }

        public async UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            await _initCurrentTime(playerProgress);
        }

        public void GameUpdate()
        {
            _updateCurrentTime();

            _updateTickTime();
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.GameExitTime = _currentTime;
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

        private async UniTask _initCurrentTime(PlayerProgressData playerProgressData)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get("https://www.google.com");

            if (webRequest is { result: UnityWebRequest.Result.Success })
            {
                await webRequest.SendWebRequest();

                string netTime = webRequest.GetResponseHeader("date");
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

#if DEBUGGING
            Debugging.Log(this, $"[_initCurrentTime] End init.\n" +
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