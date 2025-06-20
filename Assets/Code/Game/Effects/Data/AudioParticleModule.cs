﻿using System;
using Code.Data;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.LoopbackAudio;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Effects
{
    public sealed class AudioParticleModule : MonoBehaviour, IWindowsSpecific, 
        IInitializeListener, 
        IStartListener, 
        IUpdateListener
    {
        private enum LoopBackAudioParamType
        {
            None,
            ScaledMax,
            ScaledEnergy
        }

        [Serializable]
        private struct Data
        {
            public EParticleParamType ParticleParam;
            public LoopBackAudioParamType AudioParam;
            public float Multiplier;
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
            _loopbackAudioService = Container.Instance.GetService<LoopbackAudioService>();
            
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
                Log.Info(this, $"update effect {effect.ParticleParam}");
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
                    Log.Info($"{_particleSystem.Type} sleep -> {effect.ParticleParam}", Log.Type.VFX);
                    return false;
                }
            }

            return true;
        }

        public void On()
        {
            _isActive = true;
            _enabledTime = 0;
            Log.Info($"On {_particleSystem.Type}", Log.Type.VFX);
        }

        public void Off()
        {
            _isActive = false;
            Log.Info($"Off {_particleSystem.Type}", Log.Type.VFX);
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