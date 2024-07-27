using System;
using System.Collections.Generic;
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
            [MinMaxRangeFloat(0,50)] public RangedFloat Range;
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


            foreach (var effect in _effectsData)
            {
                Refresh(effect);
            }
        }

        private void Refresh(Data effect)
        {
            if (!_isUsed)
            {
                return;
            }

            switch (effect.ParticleParam)
            {
                case ParticleParamType.None:
                default:
                    break;
                case ParticleParamType.SizeMultiplier:
                    _particleSystem.SetSizeMultiplier(GetValue(effect));
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
                    _particleSystem.SetLifetime(GetValue(effect));
                    break;
            }
        }

        public virtual void On()
        {
            if (!_isUsed)
            {
                return;
            }

            _isActive = true;
        }

        public virtual void Off()
        {
            if (!_isUsed)
            {
                return;
            }

            _isActive = false;
            foreach (var effect in _effectsData)
            {
                Reset(effect);
            }
        }

        private void Reset(Data effect)
        {
            if (!_isUsed)
            {
                return;
            }

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


        private float GetValue(Data effect)
        {
            if (!_isUsed)
            {
                return 0;
            }

            float value;
            switch (effect.AudioParam)
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

            return Mathf.Clamp(value * effect.Multiplier, effect.Range.MinValue, effect.Range.MaxValue);
        }

        private float GetValue(LoopBackAudioParamType effectAudioParam, float effectMultiplier)
        {
            if (!_isUsed)
            {
                return 0;
            }

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