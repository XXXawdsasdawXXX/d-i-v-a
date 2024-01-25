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

        public void Stop(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        public void Stop(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        
        public void GameExit()
        {
            StopAllCoroutines();
        }
    }
}