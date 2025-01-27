using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Infrastructure.CustomActions
{
    [Preserve]
    public class CustomAction_StarrySky : CustomAction, ISubscriber, IStartListener
    {
        private readonly TimeObserver _timeObserver;
        private readonly ParticleSystemFacade _skyStarsParticle;

        public CustomAction_StarrySky()
        {
            ParticlesStorage particleDictionary = Container.Instance.FindStorage<ParticlesStorage>();
           
            if (particleDictionary.TryGetParticle(EParticleType.StarrySky, out ParticleSystemFacade[] skyStarsParticle))
            {
                _timeObserver = Container.Instance.FindService<TimeObserver>();
                _skyStarsParticle = skyStarsParticle[0];
            }
        }

        public UniTask Subscribe()
        {
            _timeObserver.OnNightStarted += TryStartAction;
            _timeObserver.OnDayStarted += StopAction;

            return UniTask.CompletedTask;
        }

        public UniTask GameStart()
        {
            if (_timeObserver.IsNightTime())
            {
                TryStartAction();
            }

            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _timeObserver.OnNightStarted -= TryStartAction;
            _timeObserver.OnDayStarted -= StopAction;
        }
        
        protected override void TryStartAction()
        {
            _skyStarsParticle.On();
        }

        protected override void StopAction()
        {
            _skyStarsParticle.Off();
            EndCustomActionEvent?.Invoke(this);
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.StarrySky;
        }
    }
}