using System;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services.LoopbackAudio.Audio;
using Code.Utils;
using UnityEngine;

namespace Code.Test
{
    public class AudioParticleModule : MonoBehaviour, IGameInitListener, IGameTickListener, IGameStartListener
    {
        private enum LoopBackAudioParamType
        {
            None,
            ScaledMax,
            ScaledEnergy
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
        [SerializeField] private bool _isUsed;
        [SerializeField] private bool _isActive;
        [SerializeField] private float _disableSpeed = 1;
       
        private float _enabledTime;

        [Serializable]
        private class Data
        {
            public ParticleParamType ParticleParam;
            public LoopBackAudioParamType AudioParam;
            public float Multiplier = 1;
            [MinMaxRangeFloat(0, 50)] public RangedFloat Range;
        }

        public void GameInit()
        {
            if (_isUsed && Extensions.IsMacOs())
            {
                _isUsed = false;
                return;
            }
            
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }

        public void GameStart()
        {
            foreach (var effect in _effectsData)
            {
                SetMinValues(effect);
            }
        }

        public void GameTick()
        {
            if (!_isUsed)
            {
                return;
            }

            _enabledTime += Time.deltaTime;
            foreach (var effect in _effectsData)
            {
                Refresh(effect);
            }
        }

        public bool IsSleep()
        {
            foreach (var effect in _effectsData)
            {
                var value = _particleSystem.GetValue(effect.ParticleParam);
                if (value > effect.Range.MinValue)
                {
                    Debugging.Instance.Log($"{_particleSystem.Type} sleep -> {effect.ParticleParam}", Debugging.Type.VFX);
                    return false;
                }
            }

            return true;
        }

        public virtual void On()
        {
            if (!_isUsed)
            {
                return;
            }

            _isActive = true;
            _enabledTime = 0;
            Debugging.Instance.Log($"On {_particleSystem.Type}", Debugging.Type.VFX);
        }

        public virtual void Off()
        {
            if (!_isUsed)
            {
                return;
            }

            _isActive = false;
            Debugging.Instance.Log($"Off {_particleSystem.Type}", Debugging.Type.VFX);
        }

        private void Refresh(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetMainStartSizeMultiplier(GetValue(effect));
                    break;
                case ParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(GetValue(effect));
                    break;
                case ParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(GetValue(effect));
                    break;
                case ParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(GetValue(effect));
                    break;
                case ParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(GetValue(effect));
                    break;
                case ParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(GetValue(effect), _gradient);
                    break;
                case ParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(GetValue(effect), _gradient);
                    break;
                case ParticleParamType.LiveTime:
                    _particleSystem.SetMainLifetime(GetValue(effect));
                    break;
            }
        }

        private void SetMinValues(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetMainStartSizeMultiplier(effect.Range.MinValue);
                    break;
                case ParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(effect.Range.MinValue);
                    break;
                case ParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(effect.Range.MinValue);
                    break;
                case ParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(effect.Range.MinValue);
                    break;
                case ParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(effect.Range.MinValue);
                    break;
                case ParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(effect.Range.MinValue, _gradient);
                    break;
                case ParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(effect.Range.MinValue, _gradient);
                    break;
                case ParticleParamType.LiveTime:
                    _particleSystem.SetMainLifetime(effect.Range.MinValue);
                    break;
            }

        }

        private float GetValue(Data effect)
        {
            if (!_isUsed)
            {
                return 0;
            }

            var currentValue = _particleSystem.GetValue(effect.ParticleParam);
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
