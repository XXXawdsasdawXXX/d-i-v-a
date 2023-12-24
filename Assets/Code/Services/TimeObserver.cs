using System;
using System.Collections;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Services
{
    public class TimeObserver : IService,IGameStartListener, IGameTickListener
    {
        public DateTime LastSyncedLocalTime { get; private set; }
        public DateTime LastSyncedServerTime;
        
        private float _tickTime = 5;
        private float _currentTickCooldown;

        public event Action TickEvent;
        
        public void GameStart()
        {
            var coroutineController = Container.Instance.FindService<CoroutineRunner>();
            if (coroutineController == null)
            {
                Debugging.Instance.Log($"time observer can't find coroutine runner", Debugging.Type.Time);
                return;
            }

            coroutineController.StartCoroutine(Sync());
            Debugging.Instance.Log($"server time {LastSyncedServerTime} || local time {LastSyncedLocalTime}", Debugging.Type.Time);
        }

        public void GameTick()
        {
            _currentTickCooldown += Time.deltaTime;
            if (_currentTickCooldown >= _tickTime)
            {
                _currentTickCooldown = 0;
                TickEvent?.Invoke();
            }
        }

        public DateTime InterpolatedUtcNow
        {
            get { return DateTime.UtcNow + (LastSyncedServerTime - LastSyncedLocalTime); }
        }

        public IEnumerator Sync()
        {
            LastSyncedLocalTime = DateTime.UtcNow;
            
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.google.com");
            if (myHttpWebRequest == null)
            {
                yield break;
            }
            yield return myHttpWebRequest.Send();
            string netTime = myHttpWebRequest.GetResponseHeader("date");
            Debugging.Instance.Log($"net time {netTime}", Debugging.Type.Time);
            DateTime.TryParse(netTime, out LastSyncedServerTime);
        }
    }
}