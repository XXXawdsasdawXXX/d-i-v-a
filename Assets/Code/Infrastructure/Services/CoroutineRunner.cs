using System;
using System.Collections;
using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class CoroutineRunner : MonoBehaviour, IService, IGameExitListener
    {
        public void GameExit()
        {
            StopAllCoroutines();
        }

        public Coroutine StartRoutine(IEnumerator coroutine)
        {
            return coroutine == null ? null : StartCoroutine(coroutine);
        }

        public void StartActionWithDelay(Action action, float delay)
        {
            StartCoroutine(StartActionWithDelayRoutine(action, delay));
        }

        public void StopRoutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator StartActionWithDelayRoutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}