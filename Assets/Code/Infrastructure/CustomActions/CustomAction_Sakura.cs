using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Interactions;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Sakura : CustomAction, IProgressWriter, 
        IInitListener,  
        IUpdateListener, 
        IExitListener
    {
        private const float MAX_ACTIVE_MIN = 20;
        private const float NEEDED_ABSENCE_SEC = 60 * 60;

        private Interaction_ReturnAfterAbsence _interaction_returnAfterAbsence;
        private TimeObserver _timeObserver;

        private bool _isReviewed;
        private float _currentActiveSec;

        private ParticleSystemFacade[] _particleSystems;

        public void GameInitialize()
        {
            ParticlesStorage particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
            if (particleStorage.TryGetParticle(EParticleType.Sakura, out _particleSystems))
            {
                _interaction_returnAfterAbsence =
                    Container.Instance.FindInteractionObserver<Interaction_ReturnAfterAbsence>();
                _timeObserver = Container.Instance.FindService<TimeObserver>();
                SubscribeToEvents(true);
            }
        }

        public void GameUpdate()
        {
            if (IsActive)
            {
                _currentActiveSec += Time.deltaTime;
                if (_currentActiveSec >= MAX_ACTIVE_MIN * 60)
                {
                    StopAction();
                }
            }
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        protected override void TryStartAction()
        {
            _isReviewed = true;
            foreach (ParticleSystemFacade particleSystem in _particleSystems)
            {
                particleSystem.On();
            }
            base.TryStartAction();
        }

        protected override void StopAction()
        {
            foreach (ParticleSystemFacade particleSystem in _particleSystems)
            {
                particleSystem.Off();
            }

            base.StopAction();
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Sakura;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.OnTimeInitialized += _onTimeInitialized;
                _interaction_returnAfterAbsence.UserReturnEvent += TryStartAction;
            }
            else
            {
                _timeObserver.OnTimeInitialized -= _onTimeInitialized;
                _interaction_returnAfterAbsence.UserReturnEvent -= TryStartAction;
            }
        }

        private void TryStartAction(float absenceSecond)
        {
            if (_isReviewed || _timeObserver.IsNightTime() || absenceSecond < NEEDED_ABSENCE_SEC)
            {
                return;
            }

            if (Random.Range(0, 101) > 70)
            {
                TryStartAction();
            }
        }

        private void _onTimeInitialized(bool isFirstVisit)
        {
            if (_isReviewed && isFirstVisit)
            {
                _isReviewed = false;
            }
        }
        
        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _isReviewed = playerProgress.CustomActions.IsReviewedSakura;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.CustomActions.IsReviewedSakura = _isReviewed;
        }
    }
}