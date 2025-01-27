using System.Collections;
using System.Linq;
using Code.BehaviorTree.Diva;
using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    [Preserve]
    public class CustomAction_Nimbus : CustomAction_AudioParticle, ISubscriber
    {
        private DivaAnimationAnalytic _animationAnalytic;
        private DivaCondition _divaCondition;
        private InteractionStorage _interactionStorage;
        private CoroutineRunner _coroutineRunner;
        private ParticleSystemFacade _currentNimbus;
        private Coroutine _activateCoroutine;

        private readonly RangedFloat _speedRange = new() { MinValue = 3, MaxValue = 20 };
        private float _currentMoveSpeed;

        protected override UniTask InitializeCustomAction()
        {
            _animationAnalytic = _diva.FindCharacterComponent<DivaAnimationAnalytic>();
            
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            _divaCondition = Container.Instance.FindService<DivaCondition>();
            
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
             
            return base.InitializeCustomAction();
        }

        public UniTask Subscribe()
        {
            _interactionStorage.OnSwitchDominationType += _onSwitchInteractionType;
            _animationAnalytic.OnSwitchState += _onSwitchAnimationState;
            
            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _interactionStorage.OnSwitchDominationType -= _onSwitchInteractionType;
            _animationAnalytic.OnSwitchState -= _onSwitchAnimationState;
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

        private void _onSwitchAnimationState(EDivaAnimationState divaAnimationState)
        {
            if (divaAnimationState == EDivaAnimationState.Enter)
            {
                foreach (ParticleSystemFacade particlesSystem in _particlesSystems)
                {
                    particlesSystem.transform.position = _getParticlePosition();
                }
            }

            TryStartAction();
        }


        private void _onSwitchInteractionType(EInteractionType currentType)
        {
            TryStartAction();
        }

        protected override void TryStartAction()
        {
            if (!_divaCondition.CanShowNimbus())
            {
                _stop();
                return;
            }

            EParticleType particleType = GetParticleType();

            if (_currentNimbus != null)
            {
                if (_currentNimbus.Type == particleType)
                {
                    return;
                }

                _stop();
            }

            if (_particlesSystems == null)
            {
                Debugging.LogError(this, "particle system is null");
                
                return;
            }
            
            ParticleSystemFacade particle = _particlesSystems.FirstOrDefault(p => p.Type == particleType);

            if (particle != null)
            {
                _coroutineRunner.StopRoutine(_activateCoroutine);
                _activateCoroutine = _coroutineRunner.StartRoutine(_startRoutine(particle));
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
            _moveParticles();

            if (!IsActive)
            {
                return;
            }

            if (_animationAnalytic.IsTransition)
            {
                _stop();
            }

            if (_currentMoveSpeed < _speedRange.MaxValue)
            {
                _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, _speedRange.MaxValue, 3 * Time.deltaTime);
            }
        }

        private void _moveParticles()
        {
            if (_particlesSystems == null)
            {
                return;
            }

            foreach (ParticleSystemFacade particle in _particlesSystems)
            {
                Vector3 target = _getParticlePosition();
              
                Vector3 currentPos = particle.transform.position;
                
                float distance = Vector3.Distance(currentPos, target);
                
                particle.transform.position =
                    Vector3.MoveTowards(currentPos, target, _currentMoveSpeed * distance * Time.deltaTime);
            }
        }

        private void _start(ParticleSystemFacade nimbus)
        {
            _currentMoveSpeed = _speedRange.MinValue;
        
            _currentNimbus = nimbus;
            
            _currentNimbus.On();
            
            IsActive = true;
        }
        
        //todo refactoring to unitask
        private IEnumerator _startRoutine(ParticleSystemFacade nimbus)
        {
            nimbus.TryGetAudioModule(out AudioParticleModule audioModule);
         
            yield return new WaitUntil(() => audioModule.IsSleep());
            
            _start(nimbus);
        }

        private void _stop()
        {
            if (!IsActive)
            {
                return;
            }

            if (_currentNimbus != null)
            {
                _currentNimbus.Off();
                _currentNimbus = null;
            }
            
            IsActive = false;
        }

        private Vector3 _getParticlePosition()
        {
            return _characterModeAdapter.GetWorldHeatPoint() + Vector3.up * 0.2f;
        }
        
    }
}