using System.Collections.Generic;
using Code.Game.Effects;
using Code.Game.Entities.Diva;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Game.CustomActions.AudioParticles
{
    public abstract class CustomAction_AudioParticle : CustomAction, IWindowsSpecific, IInitializeListener, IUpdateListener
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
            
            await InitializeCustomAction();
        }

        public void GameUpdate()
        {
            if (IsActive)
            {
                UpdateParticles();
            }
        }

        protected abstract EParticleType[] GetParticleTypes();

        protected void InitializeParticles()
        {
            if (_particlesSystems != null)
            {
                return;
            }
            
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
        }

        protected virtual UniTask InitializeCustomAction()
        {
            return UniTask.CompletedTask;
        }

        protected override void TryStartAction()
        {
            if (IsActive)
            {
                return;
            }
         
            Log.Info(this, $"[tryStartAction] {GetActionType()} particles count = {_particlesSystems.Length}",
                Log.Type.CustomAction);
            
            foreach (ParticleSystemFacade particle in _particlesSystems) particle.On();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.On();

            base.TryStartAction();
        }

        protected override void StopAction()
        {
            if (!IsActive)
            {
                return;
            }
            
            Log.Info(this, $"[stopAction] {GetActionType()} particles count = {_particlesSystems.Length}",
                Log.Type.CustomAction);

            foreach (ParticleSystemFacade particle in _particlesSystems) particle.Off();
            foreach (AudioParticleModule particleModule in _audioParticles) particleModule.Off();

            base.StopAction();
        }

        protected abstract void UpdateParticles();
    }
}