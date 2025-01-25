using System.Collections.Generic;
using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using Code.Utils;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public abstract class CustomAction_AudioParticle : CustomAction, IWindowsSpecific, IUpdateListener,
        IStartListener,
        IInitListener
    {
        protected ParticleSystemFacade[] _particlesSystems;
        protected DivaModeAdapter _characterModeAdapter;
        protected DivaEntity _diva;

        private readonly List<AudioParticleModule> _audioParticles = new();
        private ParticlesStorage _particleStorage;

        public void GameInitialize()
        {
            _particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
        }

        public void GameStart()
        {
            if (_particleStorage.TryGetParticles(GetParticleTypes(), out _particlesSystems))
            {
                _diva = Container.Instance.FindEntity<DivaEntity>();
                _characterModeAdapter = _diva.FindCharacterComponent<DivaModeAdapter>();
                foreach (ParticleSystemFacade particleSystem in _particlesSystems)
                {
                    if (particleSystem.TryGetAudioModule(out AudioParticleModule module))
                    {
                        _audioParticles.Add(module);
                    }
                }

                Init();
            }
        }

        protected abstract EParticleType[] GetParticleTypes();

        public void GameUpdate()
        {
            UpdateParticles();
        }

        protected virtual void Init()
        {
        }

        protected override void TryStartAction()
        {
            Debugging.Log($"Старт события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (ParticleSystemFacade particle in _particlesSystems) particle.On();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.On();

            base.TryStartAction();
        }

        protected override void StopAction()
        {
            Debugging.Log($"Стоп события {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);

            foreach (ParticleSystemFacade particle in _particlesSystems) particle.Off();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.Off();

            base.StopAction();
        }

        protected abstract void UpdateParticles();
    }
}