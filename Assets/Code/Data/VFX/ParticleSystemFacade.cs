using System;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Data.VFX
{
    public class ParticleSystemFacade : MonoBehaviour, IGameInitListener
    {
        [field: SerializeField] public ParticleType Type { get; private set; }
        [SerializeField] private ParticleSystem _particleSystem;
        
        [Header("Optional modules")] 
        [SerializeField] protected AudioParticleModule _audio;

        [Header("Modules")] 
        protected  ParticleSystem.EmissionModule _emission;
        protected  ParticleSystem.MainModule _main;
        protected  ParticleSystem.TrailModule _trails;
        protected  ParticleSystem.NoiseModule _noise;
        protected  ParticleSystem.VelocityOverLifetimeModule _velocityOverLifetime;
        protected  ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;

        [Header("Services")] 
        protected  GradientsStorage _gradientsStorage;
        
        protected readonly FacadeSettings _defaultSettings = new();

        [Serializable]
        protected class FacadeSettings
        {
            public float TrailLiveTime;
            public float LiveTime;
        }

        public void GameInit()
        {
            _emission = _particleSystem.emission;
            _main = _particleSystem.main;
            _trails = _particleSystem.trails;
            _noise = _particleSystem.noise;
            _velocityOverLifetime = _particleSystem.velocityOverLifetime;
            _colorOverLifetime = _particleSystem.colorOverLifetime;

            _defaultSettings.TrailLiveTime = _trails.lifetimeMultiplier;
            _defaultSettings.LiveTime = _main.startLifetimeMultiplier;
            
            _gradientsStorage = Container.Instance.FindStorage<GradientsStorage>();
        }

        public virtual void On()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            _audio?.On();
            
            _trails.lifetimeMultiplier = _defaultSettings.TrailLiveTime;
            _main.startLifetimeMultiplier = _defaultSettings.LiveTime;
            
            _emission.enabled = true;
        }

        public virtual  void Off()
        {
            _audio?.Off();
            
            _trails.lifetimeMultiplier = 0;
            _main.startLifetimeMultiplier =0;
            
            _emission.enabled = false;
        }

        
        public void SetTrailWidthOverTrail(float value)
        {
            _trails.widthOverTrailMultiplier = value;
        }

        public void SetMainStartSizeMultiplier(float value)
        {
            _main.startSizeMultiplier = value;
        }

        public void SetVelocitySpeed(float value)
        {
            _velocityOverLifetime.speedModifierMultiplier = value;
        }

        public void SetNoiseSize(float value)
        {
            _noise.frequency = value;
        }

        public void SetTrailsLifetimeMultiplier(float value)
        {
            _trails.lifetimeMultiplier = value;
        }

        public void SetMainLifetime(float getValue)
        {
            _main.startLifetimeMultiplier = getValue;
        }

        public void SetTrailsGradientValue(float getValue, GradientType gradientType)
        {
            if (_gradientsStorage.TryGetGradient(gradientType, out var gradientData))
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
            if (_gradientsStorage.TryGetGradient(gradientType, out var gradientData))
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

        public float GetValue(ParticleParamType paramType)
        {
            switch (paramType)
            {
                case ParticleParamType.None:
                case ParticleParamType.TrailGradient:
                case ParticleParamType.ColorLiveTime:
                default:
                    return 0;
                case ParticleParamType.SizeMultiplier:
                    return _main.startSizeMultiplier;
                case ParticleParamType.TrailWidthOverTrail:
                    return _trails.widthOverTrailMultiplier;
                case ParticleParamType.VelocitySpeed:
                    return _velocityOverLifetime.speedModifierMultiplier;
                case ParticleParamType.NoiseSize:
                    return _noise.sizeAmount.constant;
                case ParticleParamType.TrailLiveTime:
                    return _trails.lifetimeMultiplier;
                case ParticleParamType.LiveTime:
                    return _main.startLifetimeMultiplier;
            }
        }

        public bool TryGetAudioModule(out AudioParticleModule audioModule)
        {
            audioModule = _audio;
            return audioModule != null;
        }
    }
}