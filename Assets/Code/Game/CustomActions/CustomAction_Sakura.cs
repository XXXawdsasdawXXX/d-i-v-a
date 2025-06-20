﻿using Code.Game.Effects;
using Code.Game.Services.Interactions;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.CustomActions
{
    [Preserve]
    public class CustomAction_Sakura : CustomAction, IProgressWriter, IInitializeListener, IUpdateListener, ISubscriber
    {
        private const float MAX_ACTIVE_MIN = 20;
        private const float NEEDED_ABSENCE_SEC = 60 * 60;

        private Interaction_ReturnAfterAbsence _interaction_returnAfterAbsence;
        private TimeObserver _timeObserver;
        private ParticlesStorage _particleStorage;
        
        private ParticleSystemFacade[] _particleSystems;

        private bool _isReviewed;
        private float _currentActiveSec;


        public UniTask GameInitialize()
        {
            _particleStorage = Container.Instance.FindStorage<ParticlesStorage>();
            _interaction_returnAfterAbsence = Container.Instance
                .FindInteractionObserver<Interaction_ReturnAfterAbsence>();
                
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            
            return UniTask.CompletedTask;
        }
        
        public void Subscribe()
        {
            _timeObserver.OnTimeInitialized += _onTimeInitialized;
            _interaction_returnAfterAbsence.UserReturnEvent += _tryStartAction;
        }
        
        public UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            _isReviewed = playerProgress.CustomActions.IsReviewedSakura;
            
            return UniTask.CompletedTask;
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

        public void Unsubscribe()
        {
            _timeObserver.OnTimeInitialized -= _onTimeInitialized;
            _interaction_returnAfterAbsence.UserReturnEvent -= _tryStartAction;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.CustomActions.IsReviewedSakura = _isReviewed;
        }

        protected override void TryStartAction()
        {
            if (_particleSystems == null)
            {
                _particleStorage.TryGetParticle(EParticleType.Sakura, out _particleSystems);
            }
            
            _isReviewed = true;
            
            foreach (ParticleSystemFacade particleSystem in _particleSystems)
            {
                particleSystem.On();
            }
         
            base.TryStartAction();
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Sakura;
        }

        protected override void StopAction()
        {
            foreach (ParticleSystemFacade particleSystem in _particleSystems)
            {
                particleSystem.Off();
            }

            base.StopAction();
        }

        private void _tryStartAction(float absenceSecond)
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
    }
}