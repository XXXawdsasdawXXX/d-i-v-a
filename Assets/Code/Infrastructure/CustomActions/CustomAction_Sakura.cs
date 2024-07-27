using Code.Data.Enums;
using Code.Data.Facades;
using Code.Data.SavedData;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Interactions;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Sakura : CustomAction, IGameInitListener,IGameTickListener,IGameExitListener,IProgressWriter
    {
        private Interaction_ReturnAfterAbsence _interaction_returnAfterAbsence;
        private TimeObserver _timeObserver;

        private bool _isReviewed;
        private float _currentActiveSec;
        private const float MAX_ACTIVE_MIN = 60;
        private const float NEEDED_ABSENCE_SEC = 60 * 60;
        

        private ParticleSystemFacade[] _particleSystems;
        public void GameInit()
        {
            var particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
            if (particleStorage.TryGetParticle(ParticleType.Sakura, out _particleSystems))
            {
                _interaction_returnAfterAbsence =Container.Instance.FindInteractionObserver<Interaction_ReturnAfterAbsence>();
                _timeObserver = Container.Instance.FindService<TimeObserver>();
                SubscribeToEvents(true);
            }
        }

        public void GameTick()
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

        protected override void StartAction()
        {
            _isReviewed = true;
            foreach (var particleSystem in _particleSystems)
            {
                particleSystem.On();
            }
            base.StartAction();
        }

        protected override void StopAction()
        {
            foreach (var particleSystem in _particleSystems)
            {
                particleSystem.Off();
            }
            base.StopAction();
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Sakura;
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _isReviewed = playerProgress.CustomActions.IsReviewedSakura;
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.CustomActions.IsReviewedSakura = _isReviewed;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.InitTimeEvent += OnInitTimeEvent;
                _interaction_returnAfterAbsence.UserReturnEvent += TryStartAction;
            }
            else
            {
                _timeObserver.InitTimeEvent -= OnInitTimeEvent;
                _interaction_returnAfterAbsence.UserReturnEvent -= TryStartAction;
            }
        }

        private void TryStartAction(float absenceSecond)
        {
            if (_isReviewed || absenceSecond < NEEDED_ABSENCE_SEC)
            {
                return;
            }

            if (Random.Range(0, 101) > 70)
            {
                StartAction();
            }
        }

        private void OnInitTimeEvent(bool isFirstVisit)
        {
            if (_isReviewed && isFirstVisit)
            {
                _isReviewed = false;
            }
        }
    }
}