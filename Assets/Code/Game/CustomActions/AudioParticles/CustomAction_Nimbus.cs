using System.Collections;
using System.Linq;
using Code.Data;
using Code.Game.BehaviorTree.Diva;
using Code.Game.Effects;
using Code.Game.Entities.Diva;
using Code.Game.Services;
using Code.Game.Services.Interactions;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.CustomActions.AudioParticles
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
            
            _divaCondition = Container.Instance.GetService<DivaCondition>();
            
            _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
             
            return base.InitializeCustomAction();
        }

        public void Subscribe()
        {
            _interactionStorage.OnSwitchDominationType += _onSwitchInteractionType;
            _animationAnalytic.OnSwitchState += _onSwitchAnimationState;
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

        protected override void TryStartAction()
        {
            if (IsActive)
            {
                return;
            }
            
            Log.Info(this, "Try start action");
            
            InitializeParticles();
            
            if (!_divaCondition.CanShowNimbus())
            {
                _stop();
                
                return;
            }

            EParticleType particleType = _getParticleType();

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
                Log.Error(this, "particle system is null");
                
                return;
            }
            
            ParticleSystemFacade particle = _particlesSystems.FirstOrDefault(p => p.Type == particleType);

            if (particle != null)
            {
                _currentNimbus = particle;
            
                _coroutineRunner.StopRoutine(_activateCoroutine);
                _activateCoroutine = _coroutineRunner.StartRoutine(_startRoutine(particle));
                
                Log.Info(this, "Start action");
            }
        }

        protected override void UpdateParticles()
        {
            _moveParticles();

            if (_animationAnalytic.IsTransition)
            {
                _stop();
            }

            if (_currentMoveSpeed < _speedRange.MaxValue)
            {
                _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, _speedRange.MaxValue, 3 * Time.deltaTime);
            }
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

        private EParticleType _getParticleType()
        {
            return _interactionStorage.GetDominantInteractionType() == EInteractionType.Good
                ? EParticleType.Nimbus_light
                : EParticleType.Nimbus_dark;
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

            yield return null;
         //   yield return new WaitUntil(() => audioModule.IsSleep());
            
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