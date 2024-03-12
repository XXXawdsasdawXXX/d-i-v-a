using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarrySky: CustomAction
    {
        private readonly bool _isDisable;
        private readonly TimeObserver _timeObserver;
        private readonly ParticleSystemFacade _skyStarsParticle;

        public CustomAction_StarrySky() 
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (!particleDictionary.TryGetParticle(ParticleType.SkyStars, out var skyStarsParticle))
            {
                _isDisable = true;
                return;
            }

            _skyStarsParticle = skyStarsParticle[0];
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if(_isDisable)return;
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

        public override void StartAction()
        {
            if(_isDisable)return;
            _skyStarsParticle.On();
        }
        
        public override void StopAction()
        { 
            if(_isDisable)return;
            _skyStarsParticle.Off();
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarrySky;
        }
    }
}