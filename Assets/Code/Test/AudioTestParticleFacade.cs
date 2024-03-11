using System;
using System.Collections.Generic;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Test
{
    public class AudioTestParticleFacade : MonoBehaviour, IGameInitListener, IGameTickListener
    {
        private enum AudioType
        {
            None,
            ScaledMax,
            ScaledEnergy
        }

        private enum ParamType
        {
            None,
            SizeMultiplier,
            TrailWidthOverTrail,
            VelocityOverLifetime,
            NoiseSize,
            TrailLiveTime,
            TrailGradient
        }

        [SerializeField] private ParticleSystemFacade _particleSystem;
        [SerializeField] private bool _isTesting;
        [SerializeField] private List<ParamType> _params;
        [SerializeField] private AudioType _audio;
        [SerializeField] private float _valueMultiplier = 1;

        private LoopbackAudioService _loopbackAudioService;

        public void GameInit()
        {
            _loopbackAudioService = Container.Instance.FindService<LoopbackAudioService>();
        }

        public void GameTick()
        {
            if (!_isTesting)
            {
                return;
            }


            foreach (var paramType in _params)
            {
                Refresh(paramType);
            }
        }

        private void Refresh(ParamType param)
        {
            switch (param)
            {
                case ParamType.None:
                default:
                    break;
                case ParamType.SizeMultiplier:
                    _particleSystem.SetSizeMultiplier(GetValue());
                    break;
                case ParamType.TrailWidthOverTrail:
                    _particleSystem.SetTrailWidthOverTrail(GetValue());
                    break;
                case ParamType.VelocityOverLifetime:
                    _particleSystem.SetVelocityOverLifetime(GetValue());
                    break;
                case ParamType.NoiseSize:
                    _particleSystem.SetNoiseSize(GetValue());
                    break;
                case ParamType.TrailLiveTime:
                    _particleSystem.SetTrailsLifetimeMultiplier(GetValue());
                    break;
                case ParamType.TrailGradient:
                    _particleSystem.SetTrailsGradientValue(GetValue());
                    break;
            }
        }

        private float GetValue()
        {
            float value = 0;
            switch (_audio)
            {
                case AudioType.None:
                default:
                    value = 0;
                    break;
                case AudioType.ScaledMax:
                    value = _loopbackAudioService.PostScaledMax;
                    break;
                case AudioType.ScaledEnergy:
                    value = _loopbackAudioService.PostScaledEnergy;
                    break;
            }

            return value * _valueMultiplier;
        }

        private void OnValidate()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystemFacade>();
            }
        }
    }
}