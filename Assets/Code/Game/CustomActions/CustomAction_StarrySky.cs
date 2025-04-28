using Code.Game.Effects;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.CustomActions
{
    [Preserve]
    public class CustomAction_StarrySky : CustomAction, IInitializeListener, ISubscriber, IStartListener
    {
        private TimeObserver _timeObserver;
        private ParticleSystemFacade _skyStarsParticle;
        private ParticlesStorage _particleStorage;

        public UniTask GameInitialize()
        {
            _particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _timeObserver.OnNightStarted += TryStartAction;
            _timeObserver.OnDayStarted += StopAction;
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
            if (_skyStarsParticle == null)
            {
                if (_particleStorage.TryGetParticle(EParticleType.StarrySky, out ParticleSystemFacade[] skyStarsParticle))
                {
                    _skyStarsParticle = skyStarsParticle[0];
                }
            }
            
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