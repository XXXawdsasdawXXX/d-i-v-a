using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using UnityEngine;

namespace Code.Data.Facades
{
    public class ParticleSystemFacade : MonoBehaviour, IGameInitListener
    {
        [field: SerializeField] public ParticleType Type { get; private set; }
        [SerializeField] private ParticleSystem _particleSystem;
        
        [Header("Optional modules")] 
        [SerializeField] private AudioParticleModule _audio;
        
        [Header("Modules")] 
        private ParticleSystem.EmissionModule _emission;
        private ParticleSystem.MainModule _main;
        private ParticleSystem.TrailModule _trails;
        private ParticleSystem.NoiseModule _noise;
        private ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;
        private ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;
        
        [Header("Services")] 
        private GradientsDictionary _gradientsDictionary;
        
        public bool IsPlay => _particleSystem.isPlaying;

        public void GameInit()
        {
            _emission = _particleSystem.emission;
            _main = _particleSystem.main;
            _trails = _particleSystem.trails;
            _noise = _particleSystem.noise;
            _velocityOverLifetime = _particleSystem.velocityOverLifetime;
            _colorOverLifetime = _particleSystem.colorOverLifetime;
            
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

        public void SetVelocitySpeed(float value)
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
        

        public void SetTrailsGradientValue(float getValue, GradientType gradientType)
        {
            if (_gradientsDictionary.TryGetGradient(gradientType, out var gradientData))
            {
                var colors = new GradientColorKey[gradientData.colorKeys.Length];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new GradientColorKey(gradientData.colorKeys[i].color,
                        gradientData.colorKeys[i].time * Mathf.Clamp(getValue, 0, 1));
                }

                var alphas = new GradientAlphaKey[gradientData.alphaKeys.Length];
                for (int i = 0; i < alphas.Length; i++)
                {
                    alphas[i] = new GradientAlphaKey(gradientData.alphaKeys[i].alpha, i / alphas.Length);
                }

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

        public void SetLifetimeColor(float getValue, GradientType gradientType)
        {
            if (_gradientsDictionary.TryGetGradient(gradientType, out var gradientData))
            {
                var colors = new GradientColorKey[gradientData.colorKeys.Length];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = new GradientColorKey(gradientData.colorKeys[i].color,
                        gradientData.colorKeys[i].time * Mathf.Clamp(getValue, 0, 1));
                }

                var alphas = gradientData.alphaKeys;
                var newGradient = new Gradient();
                newGradient.SetKeys(colors, alphas);
                var minMaxGradient = new ParticleSystem.MinMaxGradient()
                {
                    gradient = newGradient,
                    mode = ParticleSystemGradientMode.Gradient
                };
                _colorOverLifetime.color = minMaxGradient;
            }
        }

        public bool TryGetAudioModule(out AudioParticleModule audioModule)
        {
            audioModule = _audio;
            return audioModule != null;
        }

        public void SetLifetime(float getValue)
        {
            _main.startLifetimeMultiplier = getValue;
        }
    }
}