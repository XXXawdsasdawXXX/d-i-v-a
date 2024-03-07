using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Data.Facades
{
    public class ParticleSystemFacade : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private ParticleSystem.EmissionModule _emission;
        private ParticleSystem.MainModule _main;
        private ParticleSystem.TrailModule _trails;
        private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;
        public bool IsPlay => _particleSystem.isPlaying;


        public void GameInit()
        {
            _emission = _particleSystem.emission;
            _main = _particleSystem.main;
            _trails = _particleSystem.trails;
            _velocityOverLifetime = _particleSystem.velocityOverLifetime;
          // Off();
            
        }


     
        public void SetVelocityOverLifetime(float value)
        {
            _velocityOverLifetime.speedModifier = value;
        }

        
        public void SetTrailWidthOverTrail(float value)
        {
            _trails.widthOverTrailMultiplier = value;
        }
        public void SetStartSpeed(float startSpeed)
        {
            _main.startSpeed = startSpeed;
        }

   
        public void On()
        {
            
            _emission.enabled = true;
            /*_particleSystem.gameObject.SetActive(true);
            _particleSystem.Clear();
            _particleSystem.Play();*/
        }

        public void Off()
        {
            _emission.enabled = false;
          //  _particleSystem.Stop();
        }

        public void SetColor(Color pastelColor)
        {
            _main.startColor = pastelColor;
        }
        

        public void SetSizeMultiplier(float value)
        {
            _main.startSizeMultiplier = value;
        }
    }
}