using System;
using System.Collections.Generic;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
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
            ColorLiveTime
        }

        [SerializeField] private bool _isActive;
        [Space]
        [SerializeField] private ParticleSystemFacade _particleSystem;
        [SerializeField] private List<ParticleParamType> _params;
        [SerializeField] private LoopBackAudioParamType _loopBackAudioParam;
        [SerializeField] private float _valueMultiplier = 1;
        [Header("Optional")]
        [SerializeField] private GradientType _gradient;

        private LoopbackAudioService _loopbackAudioService;

        public void GameInit()
        {
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }

        public void GameTick()
        {
            if (!_isActive)
            {
                return;
            }
            
            foreach (var paramType in _params)
            {
                Refresh(paramType);
            }
        }

        public virtual void On()
        {
            _isActive = true;
        }

        public virtual void Off()
        {
            _isActive = false;
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
        
    }
}