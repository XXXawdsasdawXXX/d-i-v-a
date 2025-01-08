using System.Collections;
using System.Linq;
using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.BehaviorTree;
using Code.Infrastructure.BehaviorTree.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Nimbus : CustomAction_AudioParticle, IGameExitListener
    {
        private DivaAnimationAnalytic _animationAnalytic;
        private DivaCondition _divaCondition;
        private InteractionStorage _interactionStorage;

        private CoroutineRunner _coroutineRunner;

        private ParticleSystemFacade _currentNimbus;

        private Coroutine _activateCoroutine;

        private RangedFloat _speedRange = new() { MinValue = 3, MaxValue = 20 };
        private float _currentMoveSpeed;

        protected override void Init()
        {
            _animationAnalytic = _diva.FindCharacterComponent<DivaAnimationAnalytic>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _divaCondition = Container.Instance.FindService<DivaCondition>();

            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();

            SubscribeToEvents(true);
            base.Init();
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Nimbus;
        }

        protected override EParticleType[] GetParticleTypes()
        {
            return new[]
            {
                EParticleType.Nimbus_light,
                EParticleType.Nimbus_dark,
            };
        }

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _interactionStorage.OnSwitchDominationType += OnSwitchInteractionType;
                _animationAnalytic.OnSwitchState += OnSwitchAnimationState;
            }
            else
            {
                _interactionStorage.OnSwitchDominationType -= OnSwitchInteractionType;
                _animationAnalytic.OnSwitchState -= OnSwitchAnimationState;
            }
        }

        private void OnSwitchAnimationState(EDivaAnimationState divaAnimationState)
        {
            if (divaAnimationState == EDivaAnimationState.Enter)
            {
                foreach (ParticleSystemFacade particlesSystem in _particlesSystems)
                {
                    particlesSystem.transform.position = GetParticlePosition();
                }
            }

            TryStartAction();
        }


        private void OnSwitchInteractionType(EInteractionType currentType)
        {
            TryStartAction();
        }

        #endregion

        protected override void TryStartAction()
        {
            if (!_divaCondition.CanShowNimbus())
            {
                Stop();
                return;
            }

            EParticleType particleType = GetParticleType();

            if (_currentNimbus != null)
            {
                if (_currentNimbus.Type == particleType)
                {
                    return;
                }

                Stop();
            }

            ParticleSystemFacade particle = _particlesSystems.FirstOrDefault(p => p.Type == particleType);

            if (particle != null)
            {
                _coroutineRunner.StopRoutine(_activateCoroutine);
                _activateCoroutine = _coroutineRunner.StartRoutine(StartRoutine(particle));
            }
        }

        private EParticleType GetParticleType()
        {
            return _interactionStorage.GetDominantInteractionType() == EInteractionType.Good
                ? EParticleType.Nimbus_light
                : EParticleType.Nimbus_dark;
        }

        protected override void UpdateParticles()
        {
            MoveParticles();

            if (!IsActive)
            {
                return;
            }

            if (_animationAnalytic.IsTransition)
            {
                Stop();
            }

            if (_currentMoveSpeed < _speedRange.MaxValue)
            {
                _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, _speedRange.MaxValue, 3 * Time.deltaTime);
            }
        }

        private void MoveParticles()
        {
            if (_particlesSystems == null)
            {
                return;
            }

            foreach (ParticleSystemFacade particle in _particlesSystems)
            {
                Vector3 target = GetParticlePosition();
                Vector3 currentPos = particle.transform.position;
                float distance = Vector3.Distance(currentPos, target);
                particle.transform.position =
                    Vector3.MoveTowards(currentPos, target, _currentMoveSpeed * distance * Time.deltaTime);
            }
        }

        private void Start(ParticleSystemFacade nimbus)
        {
            _currentMoveSpeed = _speedRange.MinValue;
            _currentNimbus = nimbus;
            _currentNimbus.On();
            IsActive = true;
        }

        private IEnumerator StartRoutine(ParticleSystemFacade nimbus)
        {
            nimbus.TryGetAudioModule(out AudioParticleModule audioModule);
            yield return new WaitUntil(() => audioModule.IsSleep());
            Start(nimbus);
        }

        private void Stop()
        {
            if (!IsActive)
            {
                return;
            }

            _currentNimbus?.Off();
            _currentNimbus = null;
            IsActive = false;
        }

        private Vector3 GetParticlePosition()
        {
            return _characterModeAdapter.GetWorldHeatPoint() + Vector3.up * 0.2f;
        }
    }
}