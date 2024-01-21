using System;
using System.Collections;
using Code.Data.Interfaces;
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
        private const float _tickTime = 5;

        public DateTime CurrentTime;
        private float _currentTickCooldown;

        private bool _isInit;

        public event Action TickEvent;
        public event Action InitTimeEvent;
        public event Action StartDayEvent;

        [Obsolete("Obsolete")]
        public void GameInit()
        {
            var coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            if (coroutineRunner == null)
            {
                Debugging.Instance.Log($"time observer can't find coroutine runner", Debugging.Type.Time);
                return;
            }

            coroutineRunner.StartRoutine(InitCurrentTime());

            Debugging.Instance.Log($"Current time {CurrentTime}", Debugging.Type.Time);
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
            CurrentTime += TimeSpan.FromSeconds(Time.deltaTime);
        }

        private void UpdateTickTime()
        {
            _currentTickCooldown += Time.deltaTime;
            if (_currentTickCooldown >= _tickTime)
            {
                _currentTickCooldown = 0;
                Debugging.Instance.Log($"Tick", Debugging.Type.Time);
                TickEvent?.Invoke();
            }
        }

        public bool IsNightTime()
        {
            TimeSpan timeOfDay = CurrentTime.TimeOfDay;
            return (timeOfDay >= NightStart || timeOfDay < NightEnd);
        }


        [Obsolete("Obsolete")]
        private IEnumerator InitCurrentTime()
        {
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.google.com");
            if (myHttpWebRequest == null)
            {
                CurrentTime = DateTime.UtcNow;
                _isInit = true;
                InitTimeEvent?.Invoke();
                yield break;
            }

            yield return myHttpWebRequest.Send();
            string netTime = myHttpWebRequest.GetResponseHeader("date");
            Debugging.Instance.Log($"net time {netTime}", Debugging.Type.Time);
            DateTime.TryParse(netTime, out CurrentTime);
            InitTimeEvent?.Invoke();
            _isInit = true;
        }
    }
}