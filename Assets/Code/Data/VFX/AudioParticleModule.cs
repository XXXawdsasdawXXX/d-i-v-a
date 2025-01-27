using System;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services.LoopbackAudio.Audio;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Data
{
    public class AudioParticleModule : MonoBehaviour, IWindowsSpecific, IInitListener, IUpdateListener, IStartListener
    {
        private enum LoopBackAudioParamType
        {
            None,
            ScaledMax,
            ScaledEnergy
        }

        [Serializable]
        private class Data
        {
            public EParticleParamType ParticleParam;
            public LoopBackAudioParamType AudioParam;
            public float Multiplier = 1;
            [MinMaxRangeFloat(0, 50)] public RangedFloat Range;
        }

        [Header("Base")] 
        [SerializeField] private ParticleSystemFacade _particleSystem;

        [Space] 
        [SerializeField] private Data[] _effectsData;

        [Header("Optional")] 
        [SerializeField] private GradientType _gradient;

        [Header("Services")] 
        private LoopbackAudioService _loopbackAudioService;

        [Header("Dynamic data")] 
        [SerializeField] private bool _isActive;
        [SerializeField] private float _disableSpeed = 1;

        private float _enabledTime;

        public UniTask GameInitialize()
        {
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
            
            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            foreach (Data effect in _effectsData)
            {
                SetMinValues(effect);
            }
            
            return UniTask.CompletedTask;
        }

        public void GameUpdate()
        {
            _enabledTime += Time.deltaTime;
            foreach (Data effect in _effectsData)
            {
                _refresh(effect);
            }
        }

        public bool IsSleep()
        {
            foreach (Data effect in _effectsData)
            {
                float value = _particleSystem.GetValue(effect.ParticleParam);
                if (value > effect.Range.MinValue)
                {
                    Debugging.Log($"{_particleSystem.Type} sleep -> {effect.ParticleParam}",
                        Debugging.Type.VFX);
                    return false;
                }
            }

            return true;
        }

        public virtual void On()
        {
            _isActive = true;
            _enabledTime = 0;
#if DEBUGGING
            Debugging.Log($"On {_particleSystem.Type}", Debugging.Type.VFX);
#endif
        }

        public virtual void Off()
        {
            _isActive = false;
#if DEBUGGING
            Debugging.Log($"Off {_particleSystem.Type}", Debugging.Type.VFX);
#endif
        }

        private void _refresh(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case EParticleParamType.None:
                default:
                    break;
              
                case EParticleParamType.SizeMultiplier:
                    _particleSystem.SetMainStartSizeMultiplier(GetValue(effect));
                    break;
                
                case EParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(GetValue(effect));
                    break;
                
                case EParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(GetValue(effect));
                    break;
                
                case EParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(GetValue(effect));
                    break;
                
                case EParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(GetValue(effect));
                    break;
                
                case EParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(GetValue(effect), _gradient);
                    break;
                
                case EParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(GetValue(effect), _gradient);
                    break;
                
                case EParticleParamType.LiveTime:
                    _particleSystem.SetMainLifetime(GetValue(effect));
                    break;
            }
        }

        private void SetMinValues(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case EParticleParamType.None:
                default:
                    break;
               
                case EParticleParamType.SizeMultiplier:
                    _particleSystem.SetMainStartSizeMultiplier(effect.Range.MinValue);
                    break;
                
                case EParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(effect.Range.MinValue);
                    break;
                
                case EParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(effect.Range.MinValue);
                    break;
                
                case EParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(effect.Range.MinValue);
                    break;
                
                case EParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(effect.Range.MinValue);
                    break;
                
                case EParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(effect.Range.MinValue, _gradient);
                    break;
                
                case EParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(effect.Range.MinValue, _gradient);
                    break;
                
                case EParticleParamType.LiveTime:
                    _particleSystem.SetMainLifetime(effect.Range.MinValue);
                    break;
            }
        }

        private float GetValue(Data effect)
        {
            float currentValue = _particleSystem.GetValue(effect.ParticleParam);
            float targetValue;

            switch (effect.AudioParam)
            {
                case LoopBackAudioParamType.None:
                default:
                    targetValue = 0;
                    break;
            
                case LoopBackAudioParamType.ScaledMax:
                    targetValue = _loopbackAudioService.PostScaledMax;
                    break;
                
                case LoopBackAudioParamType.ScaledEnergy:
                    targetValue = _loopbackAudioService.PostScaledEnergy;
                    break;
            }

            if (_isActive)
            {
                targetValue = Mathf.Clamp(targetValue * effect.Multiplier, effect.Range.MinValue, effect.Range.MaxValue);
            
                targetValue = Mathf.MoveTowards(currentValue, targetValue, _enabledTime * Time.deltaTime);
            }
            else
            {
                if (currentValue > effect.Range.MinValue)
                {
                    targetValue = Mathf.MoveTowards(currentValue, effect.Range.MinValue, _disableSpeed * Time.deltaTime);
                    
                    return targetValue;
                }
                else
                {
                    targetValue = effect.Range.MinValue;
                }
            }

            return targetValue;
        }
    }
}