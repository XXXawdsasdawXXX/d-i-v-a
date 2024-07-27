using Code.Data.Enums;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Utils;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarrySky : CustomAction
    {
        private readonly bool _isNotUsed;
        private readonly TimeObserver _timeObserver;
        private readonly ParticleSystemFacade _skyStarsParticle;

        public CustomAction_StarrySky()
        {
            var particleDictionary = Container.Instance.FindStorage<ParticlesStorage>();
            if (particleDictionary.TryGetParticle(ParticleType.StarrySky, out var skyStarsParticle))
            {
                _timeObserver = Container.Instance.FindService<TimeObserver>();
                _skyStarsParticle = skyStarsParticle[0];
                SubscribeToEvents(true);
                return;
            }
            _isNotUsed = true;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (_isNotUsed) return;
            if (flag)
            {
                _timeObserver.StartNightEvent += StartAction;
                _timeObserver.StartDayEvent += StopAction;
            }
            else
            {
                _timeObserver.StartNightEvent -= StartAction;
                _timeObserver.StartDayEvent -= StopAction;
            }
        }

        protected  override void StartAction()
        {
            if (_isNotUsed) return;
            _skyStarsParticle.On();
        }

        protected  override void StopAction()
        {
            if (_isNotUsed) return;
            _skyStarsParticle.Off();
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarrySky;
        }
    }
}