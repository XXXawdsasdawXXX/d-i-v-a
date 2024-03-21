using System.Collections.Generic;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Test;
using Code.Utils;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public abstract class CustomAction_AudioParticle : CustomAction, IGameTickListener, IGameStartListener,
        IGameInitListener
    {
        protected bool _isUsed = true;

        protected ParticleSystemFacade[] _particlesSystems;
        protected CharacterModeAdapter _characterModeAdapter;
        protected DIVA _diva;

        private readonly List<AudioParticleModule> _audioParticles = new();
        private ParticlesStorage _particleStorage;

        public void GameInit()
        {
            _particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
        }

        public void GameStart()
        {
            if (!_isUsed)
            {
                return;
            }

            if (_particleStorage.TryGetParticles(GetParticleTypes(), out _particlesSystems))
            {
                _diva = Container.Instance.FindEntity<DIVA>();
                _characterModeAdapter = _diva.FindCharacterComponent<CharacterModeAdapter>();
                foreach (var particleSystem in _particlesSystems)
                {
                    if (particleSystem.TryGetAudioModule(out AudioParticleModule module))
                    {
                        _audioParticles.Add(module);
                    }
                }

                Init();
                return;
            }

            _isUsed = false;
        }

        protected abstract ParticleType[] GetParticleTypes();

        public void GameTick()
        {
            if (!_isUsed)
            {
                return;
            }

            UpdateParticles();
        }


        protected virtual void Init()
        {
        }

        protected override void StartAction()
        {
            if (!_isUsed)
            {
                return;
            }

            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (var particle in _particlesSystems) particle.On();
            foreach (var particleModule in _audioParticles) particleModule.On();

            base.StartAction();
        }

        protected override void StopAction()
        {
            if (!_isUsed)
            {
                return;
            }

            Debugging.Instance.Log($"Стоп события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (var particle in _particlesSystems) particle.Off();
            foreach (var particleModule in _audioParticles) particleModule.Off();

            base.StopAction();
        }

        protected abstract void UpdateParticles();
    }
}