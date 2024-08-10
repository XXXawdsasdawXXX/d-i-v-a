using System.Collections.Generic;
using Code.Components.Entities.Characters;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.VFX;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using Code.Utils;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public abstract class CustomAction_AudioParticle : CustomAction, IWindowsSpecific, IGameTickListener, IGameStartListener,
        IGameInitListener
    {
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
            }
        }

        protected abstract ParticleType[] GetParticleTypes();

        public void GameTick()
        {
            UpdateParticles();
        }
        
        protected virtual void Init()
        {
        }

        protected override void TryStartAction()
        {
            Debugging.Instance.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (var particle in _particlesSystems) particle.On();
            foreach (var particleModule in _audioParticles) particleModule.On();

            base.TryStartAction();
        }

        protected override void StopAction()
        {
            Debugging.Instance.Log($"Стоп события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (var particle in _particlesSystems) particle.Off();
            foreach (var particleModule in _audioParticles) particleModule.Off();

            base.StopAction();
        }

        protected abstract void UpdateParticles();
    }
}