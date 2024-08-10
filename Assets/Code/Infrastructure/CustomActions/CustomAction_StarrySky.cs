using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.VFX;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarrySky : CustomAction, IGameStartListener, IGameExitListener
    {
        private readonly TimeObserver _timeObserver;
        private readonly ParticleSystemFacade _skyStarsParticle;

        public CustomAction_StarrySky()
        {
            var particleDictionary = Container.Instance.FindStorage<ParticlesStorage>();
            if (particleDictionary.TryGetParticle(ParticleType.StarrySky, out var skyStarsParticle))
            {
                _timeObserver = Container.Instance.FindService<TimeObserver>();
                _skyStarsParticle = skyStarsParticle[0];
            }
        }

        public void GameStart()
        {
            if (_timeObserver.IsNightTime())
            {
                TryStartAction();
            }

            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }
        
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.StartNightEvent += TryStartAction;
                _timeObserver.StartDayEvent += StopAction;
            }
            else
            {
                _timeObserver.StartNightEvent -= TryStartAction;
                _timeObserver.StartDayEvent -= StopAction;
            }
        }

        protected  override void TryStartAction()
        {
            _skyStarsParticle.On();
        }

        protected  override void StopAction()
        {
            _skyStarsParticle.Off();
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarrySky;
        }
    }
}