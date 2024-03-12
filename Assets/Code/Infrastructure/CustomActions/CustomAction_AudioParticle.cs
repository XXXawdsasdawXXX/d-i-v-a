using System.Collections.Generic;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Test;
using Code.Utils;

namespace Code.Infrastructure.CustomActions
{
    public abstract class CustomAction_AudioParticle : CustomAction, IGameTickListener, IGameStartListener
    {
        protected readonly bool _isNotUsed;

        protected readonly CharacterModeAdapter _characterModeAdapter;
        protected readonly ParticleSystemFacade[] _particlesSystems;
        private readonly List<AudioParticleModule> _audioParticles = new();

        protected CustomAction_AudioParticle()
        {
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (particleDictionary.TryGetParticle(GetParticleType(), out _particlesSystems))
            {
                var diva = Container.Instance.FindEntity<DIVA>();
                _characterModeAdapter = diva.FindCharacterComponent<CharacterModeAdapter>();
                foreach (var particleSystem in _particlesSystems)
                {
                    if (particleSystem.TryGetComponent(out AudioParticleModule module))
                    {
                        _audioParticles.Add(module);
                    }
                }

                Init();
                return;
            }

            _isNotUsed = true;
        }

        public void GameStart()
        {
            StartAction();
        }

        public void GameTick()
        {
            UpdateParticles();
        }

        protected abstract ParticleType GetParticleType();

        protected virtual void Init()
        {
        }

        protected  override void StartAction()
        {
            if (_isNotUsed) return;
            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}", Debugging.Type.CustomAction);
            foreach (var particle in _particlesSystems) particle.On();
            foreach (var particleModule in _audioParticles) particleModule.On();
        }

        protected override void StopAction()
        {
            if (_isNotUsed) return;
            Debugging.Instance.Log($"Стоп события {GetActionType()} particles count = {_particlesSystems.Length}", Debugging.Type.CustomAction);
            foreach (var particle in _particlesSystems) particle.Off();
            foreach (var particleModule in _audioParticles) particleModule.Off();
        }

        protected abstract void UpdateParticles();
    }
}