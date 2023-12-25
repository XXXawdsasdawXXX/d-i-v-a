using System.Collections;
using Code.Data.Interfaces;
using UnityEngine;

namespace Code.Services
{
    public class CoroutineRunner : MonoBehaviour, IService
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
    }
}