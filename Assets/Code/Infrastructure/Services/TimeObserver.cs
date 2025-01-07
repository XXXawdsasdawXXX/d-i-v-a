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

            Debugging.Log(this, $"[init] Current time {_currentTime}", Debugging.Type.Time);
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
          
            Debugging.Log(this, $"[load]", Debugging.Type.Time);
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.GameExitTime = _currentTime;
            
            Debugging.Log(this, $"[save]", Debugging.Type.Time);
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
                
                Debugging.Log(this, "Tick", Debugging.Type.Time);
                
                TickEvent?.Invoke();
                
                _checkTimeOfDay();
            }
        }

        private void _checkTimeOfDay()
        {
            bool isNightTime = IsNightTime();
        
            if (isNightTime && !_isNight)
            {
                Debugging.Log(this, "Start night", Debugging.Type.Time);
              
                _isNight = true;
                
                StartNightEvent?.Invoke();
            }
            else if (!isNightTime && _isNight)
            {
                Debugging.Log($"start day", Debugging.Type.Time);
                
                _isNight = false;
                
                StartDayEvent?.Invoke();
            }
        }

        private IEnumerator _initCurrentTime(PlayerProgressData playerProgressData)
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.google.com");
         
            if (myHttpWebRequest != null)
            {
                yield return myHttpWebRequest.SendWebRequest();
                
                string netTime = myHttpWebRequest.GetResponseHeader("date");
                
                Debugging.Log($"init google time", Debugging.Type.Time);
                
                if (!DateTime.TryParse(netTime, out _currentTime))
                {
                     Debugging.Log($"lose google time parsing", Debugging.Type.Time);
                    
                     _currentTime = DateTime.UtcNow;
                }
            }
            else
            {
                _currentTime = DateTime.UtcNow;
         
                Debugging.Log($"init standalone time", Debugging.Type.Time);
            }

            DateTime lastVisit = playerProgressData.GameEnterTime;
        
            playerProgressData.GameEnterTime = _currentTime;

            _checkTimeOfDay();

            _isInit = true;

            Debugging.Log($"End init: is first visit {!Extensions.IsEqualDay(lastVisit, _currentTime)}" +
                          $"\ncurrent {_currentTime} saving {lastVisit}", Debugging.Type.Time);

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