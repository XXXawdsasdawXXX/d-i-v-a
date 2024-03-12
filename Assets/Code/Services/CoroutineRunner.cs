using System;
using System.Collections;
using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Services
{
    public class CoroutineRunner : MonoBehaviour, IService , IGameExitListener
    {
        public Coroutine StartRoutine(IEnumerator coroutine)
        {
            return coroutine == null ? null : StartCoroutine(coroutine);
        }

        public void StartActionWithDelay(Action action, float delay)
        {
            StartCoroutine(StartActionWithDelayRoutine(action,delay));
        }
        public void StopRoutine(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        public void StopRoutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        public void GameExit()
        {
            StopAllCoroutines();
        }

        private IEnumerator StartActionWithDelayRoutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        
    }
}