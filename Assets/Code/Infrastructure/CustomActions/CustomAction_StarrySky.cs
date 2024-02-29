using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_StarrySky: CustomAction
    {
        private readonly TimeObserver _timeObserver;
        private readonly ParticleSystem _skyStarsParticle;

        public CustomAction_StarrySky() 
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            var particleDictionary = Container.Instance.FindService<ParticlesDictionary>();
            if (!particleDictionary.TryGetParticle(ParticleType.SkyStars, out _skyStarsParticle))
            {
                Debugging.Instance.ErrorLog($"Партикл по типу {ParticleType.SkyStars} не добавлен в библиотеку партиклов");
            }
            SubscribeToEvents(true);
        }

        private void SubscribeToEvents(bool flag)
        {
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
            _skyStarsParticle.Play();
        }
        
        public override void StopAction()
        { 
            _skyStarsParticle.Stop();
            EndCustomActionEvent?.Invoke(this);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.StarrySky;
        }
    }
}