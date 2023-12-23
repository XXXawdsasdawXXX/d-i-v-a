using System.Collections;
using UnityEngine;

namespace Code.Services
{
    public class CoroutineController : MonoBehaviour, IService
    {
        public void Play(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        public void Stop(IEnumerator coroutine)
        {
            StopCoroutine(coroutine);
        }
        
    }
}