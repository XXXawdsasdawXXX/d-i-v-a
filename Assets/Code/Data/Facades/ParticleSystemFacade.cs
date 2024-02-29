using UnityEngine;

namespace Code.Data.Facades
{
    public class ParticleSystemFacade : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private ParticleSystem.EmissionModule _emission;

        private void Awake()
        {
            _emission = _particleSystem.emission;
            Off();
        }
        
        public void Play()
        {
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Clear();
            _particleSystem.Play();
        }

        public void Stop()
        {
            _particleSystem.Stop();
        }

        public void On()
        {
            _emission.enabled = true;
        }

        public void Off()
        {
            _emission.enabled = false;
        }
    }
}