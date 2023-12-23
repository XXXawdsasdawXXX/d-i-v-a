using System;
using System.Collections;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using Code.Utils;
using UnityEngine.Networking;

namespace Code.Services
{
    public class TimeObserver : IService, IGameStartListener
    {
        public DateTime LastSyncedLocalTime { get; private set; }

        public DateTime LastSyncedServerTime;

        private CoroutineController _coroutineController;

        public void GameStart()
        {
            var testDrive = Container.Instance.FindService<TestDrive>();
            Debugging.Instance.Log($"Test drive {testDrive  != null}", Debugging.Type.Time);


            _coroutineController = Container.Instance.FindService<CoroutineController>();
            //var coroutineRunner = Container.Instance.GetCoroutineRunner();
            Debugging.Instance.Log($"Coroutime runner {_coroutineController != null}", Debugging.Type.Time);

            if (_coroutineController == null)
            {
                Debugging.Instance.Log($"time observer can't find coroutine runner", Debugging.Type.Time);
                return;
            }

            _coroutineController.StartCoroutine(Sync());
            Debugging.Instance.Log($"server time {LastSyncedServerTime} || local time {LastSyncedLocalTime}",
                Debugging.Type.Time);
        }

        public DateTime InterpolatedUtcNow
        {
            get { return DateTime.UtcNow + (LastSyncedServerTime - LastSyncedLocalTime); }
        }

        public IEnumerator Sync()
        {
            LastSyncedLocalTime = DateTime.UtcNow;
            
            UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.microsoft.com");
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