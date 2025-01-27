using System.Collections.Generic;
using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Test;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public abstract class CustomAction_AudioParticle : CustomAction, IWindowsSpecific, IInitListener, IUpdateListener
    {
        protected ParticleSystemFacade[] _particlesSystems;
        protected DivaModeAdapter _characterModeAdapter;
        protected DivaEntity _diva;

        private readonly List<AudioParticleModule> _audioParticles = new();
        private ParticlesStorage _particleStorage;

        public async UniTask GameInitialize()
        {
            _particleStorage = Container.Instance.FindStorage<ParticlesStorage>();

            _diva = Container.Instance.FindEntity<DivaEntity>();
            
            _characterModeAdapter = _diva.FindCharacterComponent<DivaModeAdapter>();
            
            if (_particleStorage.TryGetParticles(GetParticleTypes(), out _particlesSystems))
            {
                foreach (ParticleSystemFacade particleSystem in _particlesSystems)
                {
                    if (particleSystem.TryGetAudioModule(out AudioParticleModule module))
                    {
                        _audioParticles.Add(module);
                    }
                }
            }

            await InitializeCustomAction();
        }

        public void GameUpdate()
        {
            UpdateParticles();
        }

        protected abstract EParticleType[] GetParticleTypes();

        protected virtual UniTask InitializeCustomAction()
        {
            return UniTask.CompletedTask;
        }

        protected override void TryStartAction()
        {
#if DEBUGGING
            Debugging.Log(this, $"[tryStartAction] {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);
#endif

            foreach (ParticleSystemFacade particle in _particlesSystems) particle.On();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.On();

            base.TryStartAction();
        }

        protected override void StopAction()
        {
#if DEBUGGING
            Debugging.Log(this, $"[stopAction] {GetActionType()} particles count = {_particlesSystems.Length}",
                Debugging.Type.CustomAction);
#endif

            foreach (ParticleSystemFacade particle in _particlesSystems) particle.Off();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.Off();

            base.StopAction();
        }

        protected abstract void UpdateParticles();
    }
}