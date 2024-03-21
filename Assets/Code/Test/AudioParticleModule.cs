using System;
using System.Collections.Generic;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services.LoopbackAudio.Audio;
using Code.Utils;
using UnityEngine;

namespace Code.Test
{
    public class AudioParticleModule : MonoBehaviour, IGameInitListener, IGameTickListener
    {
        private enum LoopBackAudioParamType
        {
            None,
            ScaledMax,
            ScaledEnergy
        }

        private enum ParticleParamType
        {
            None,
            SizeMultiplier,
            TrailWidthOverTrail,
            VelocitySpeed,
            NoiseSize,
            TrailLiveTime,
            TrailGradient,
            ColorLiveTime,
            LiveTime
        }

        [Header("Base")]
        [SerializeField] private ParticleSystemFacade _particleSystem;
        [SerializeField] private List<ParticleParamType> _params;
        [SerializeField] private LoopBackAudioParamType _loopBackAudioParam;
        [SerializeField] private float _valueMultiplier = 1;
        [Space] 
        [SerializeField] private Data[] _effectsData;
        [Header("Optional")]
        [SerializeField] private GradientType _gradient;
        [Header("Services")]
        private LoopbackAudioService _loopbackAudioService;
        [Header("Dinamic value")]
        [SerializeField] private bool _isUsed;
        [SerializeField] private bool _isActive;

        [Serializable]
        private class Data
        {
            public ParticleParamType ParticleParam;
            public LoopBackAudioParamType AudioParam;
            public float Multiplier = 1;
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

        public void GameTick()
        {
            if (!_isUsed || !_isActive)
            {
                return;
            }
            
            foreach (var paramType in _params)
            {
                Refresh(paramType);
            }
            
            foreach (var effect in _effectsData)
            {
                Refresh(effect);
            }
        }

        private void Refresh(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetSizeMultiplier(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
                case ParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
                case ParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
                case ParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
                case ParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
                case ParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(GetValue(effect.AudioParam,effect.Multiplier),_gradient);
                    break;
                case ParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(GetValue(effect.AudioParam,effect.Multiplier),_gradient);
                    break;
                case ParticleParamType.LiveTime:
                    _particleSystem.SetLifetime(GetValue(effect.AudioParam,effect.Multiplier));
                    break;
            }
        }

     

        public virtual void On()
        {
            _isActive = true;
        }

        public virtual void Off()
        {
            _isActive = false;
            foreach (var effect in _effectsData)
            {
                Reset(effect);
            }
        }

        private void Reset(Data effect)
        {
            switch (effect.ParticleParam)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetSizeMultiplier(0);
                    break;
                case ParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(0);
                    break;
                case ParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(0);
                    break;
                case ParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(0);
                    break;
                case ParticleParamType.LiveTime:
                    _particleSystem.SetLifetime(0);
                    break;
            }
        }

        private void Refresh(ParticleParamType param)
        {
            switch (param)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetSizeMultiplier(GetValue());
                    break;
                case ParticleParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(GetValue());
                    break;
                case ParticleParamType.VelocitySpeed:
                    _particleSystem.SetVelocitySpeed(GetValue());
                    break;
                case ParticleParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(GetValue());
                    break;
                case ParticleParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(GetValue());
                    break;
                case ParticleParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(GetValue(),_gradient);
                    break;
                case ParticleParamType.ColorLiveTime:
                    _particleSystem.SetLifetimeColor(GetValue(),_gradient);
                    break;
                case ParticleParamType.LiveTime:
                    _particleSystem.SetLifetime(GetValue());
                    break;
            }
        }

        private float GetValue()
        {
            float value;
            switch (_loopBackAudioParam)
            {
                case LoopBackAudioParamType.None:
                default:
                    value = 0;
                    break;
                case LoopBackAudioParamType.ScaledMax:
                    value = _loopbackAudioService.PostScaledMax;
                    break;
                case LoopBackAudioParamType.ScaledEnergy:
                    value = _loopbackAudioService.PostScaledEnergy;
                    break;
            }

            return value * _valueMultiplier;
        }
        
        private float GetValue(LoopBackAudioParamType effectAudioParam, float effectMultiplier)
        {
            float value;
            switch (effectAudioParam)
            {
                case LoopBackAudioParamType.None:
                default:
                    value = 0;
                    break;
                case LoopBackAudioParamType.ScaledMax:
                    value = _loopbackAudioService.PostScaledMax;
                    break;
                case LoopBackAudioParamType.ScaledEnergy:
                    value = _loopbackAudioService.PostScaledEnergy;
                    break;
            }

            return value * effectMultiplier;
        }
        
    }
}