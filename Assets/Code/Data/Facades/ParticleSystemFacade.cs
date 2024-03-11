using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using UnityEngine;

namespace Code.Data.Facades
{
    [RequireComponent(typeof(AudioTestParticleFacade))]
    public class ParticleSystemFacade : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private ParticleSystem _particleSystem;

        [Header("Modules")]
        private ParticleSystem.EmissionModule _emission;
        private ParticleSystem.MainModule _main;
        private ParticleSystem.TrailModule _trails;
        private ParticleSystem.NoiseModule _noise;
        private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;
        public bool IsPlay => _particleSystem.isPlaying;

        [Header("Services")]
        private GradientsDictionary _gradientsDictionary;

        public void GameInit()
        {
            _emission = _particleSystem.emission;
            _main = _particleSystem.main;
            _trails = _particleSystem.trails;
            _noise = _particleSystem.noise;
            _velocityOverLifetime = _particleSystem.velocityOverLifetime;

            _gradientsDictionary = Container.Instance.FindService<GradientsDictionary>();
        }

        public void On()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            _emission.enabled = true;
        }

        public void Off()
        {
            _emission.enabled = false;
        }

        public void SetTrailWidthOverTrail(float value)
        {
            _trails.widthOverTrailMultiplier = value;
        }
        
        public void SetSizeMultiplier(float value)
        {
            _main.startSizeMultiplier = value;
        }
        
        public void SetVelocityOverLifetime(float value)
        {
            _velocityOverLifetime.speedModifier = value;
        }

        public void SetNoiseSize(float value)
        {
            _noise.frequency = value;
        }

        public void SetTrailsLifetimeMultiplier(float value)
        {
            _trails.lifetimeMultiplier = value;
        }

        [ContextMenu("SetTrailsGradient")]
        public void SetTrailsGradient()
        {
            _gradientsDictionary.TryGetGradient(GradientType.SoftBlue, out var blueGradient);
            var gradient = new ParticleSystem.MinMaxGradient()
            {
                gradient = blueGradient,
                mode = ParticleSystemGradientMode.Gradient
            };
            _trails.colorOverLifetime = gradient;
        }

        public void SetTrailsGradientValue(float getValue)
        {
            _gradientsDictionary.TryGetGradient(GradientType.SoftBlue, out var gradientData);
            var currentGradient = _trails.colorOverLifetime.gradient;
            var colors = new GradientColorKey[2];
            colors[0] = new GradientColorKey(gradientData.colorKeys[0].color, Mathf.Clamp(getValue,0,0.75f));
            colors[1] = new GradientColorKey(gradientData.colorKeys[1].color, 1.0f);
            var alphas = new GradientAlphaKey[2];
            alphas[0] = new GradientAlphaKey(1.0f, 0.75f);
            alphas[1] = new GradientAlphaKey(0f, 1.0f);
            var newGradient = new Gradient();
            newGradient.SetKeys(colors, alphas);
            var gradient = new ParticleSystem.MinMaxGradient()
            {
                gradient = newGradient,
                mode = ParticleSystemGradientMode.Gradient
            };
            _trails.colorOverLifetime = gradient;
        }
    }
}