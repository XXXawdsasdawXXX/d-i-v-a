using System.Collections;
using System.Linq;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Data.Storages;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.CustomNodes.Character;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Nimbus : CustomAction_AudioParticle, IGameExitListener
    {
        private CharacterAnimationAnalytic _animationAnalytic;
        private CharacterCondition _characterCondition;
        private InteractionStorage _interactionStorage;

        private CoroutineRunner _coroutineRunner;
        
        private ParticleSystemFacade _currentNimbus;

        private Coroutine _activateCoroutine;
        
        private RangedFloat _speedRange = new() { MinValue = 3, MaxValue = 20};
        private float _currentMoveSpeed;
        
        protected override void Init()
        {
            _animationAnalytic = _diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _characterCondition = Container.Instance.FindService<CharacterCondition>();

            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            
            SubscribeToEvents(true);
            base.Init();
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Nimbus;
        }

        protected override ParticleType[] GetParticleTypes()
        {
            return new[]
            {
                ParticleType.Nimbus_light,
                ParticleType.Nimbus_dark,
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

        private void OnSwitchAnimationState(CharacterAnimationState obj) => TryStartAction();


        private void OnSwitchInteractionType(InteractionType currentType) => TryStartAction();
        
        
        #endregion

        protected override void TryStartAction()
        {
            if (!_characterCondition.IsCanShowNimbus())
            {
                Disable();
                return;
            }

            var particleType = _interactionStorage.GetDominantInteractionType() == InteractionType.Good 
                ? ParticleType.Nimbus_light
                :ParticleType.Nimbus_dark;
            
            if (_currentNimbus != null)
            {
                if (_currentNimbus.Type == particleType)
                {
                    Debugging.Instance.Log(this,$"Try restars nimbus {particleType} (return)", Debugging.Type.VFX);
                    return;
                }
                else
                    Disable();
            }

            var particle = _particlesSystems.FirstOrDefault(p => p.Type == particleType);

            if (particle != null)
            {
                _coroutineRunner.StopRoutine(_activateCoroutine);
                _activateCoroutine = _coroutineRunner.StartRoutine(ActiveRoutine(particle));
            }
        }
        
        protected override void UpdateParticles()
        {
            if (!IsActive)
            {
                return;
            }
            
            if (_animationAnalytic.IsTransition)
            { 
                Disable();
            }

            if (_currentMoveSpeed < _speedRange.MaxValue)
            {
                _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, _speedRange.MaxValue, 3 * Time.deltaTime);
            }
            
            foreach (var particle in _particlesSystems)
            {
                var target = GetParticlePosition();
                var currentPos = particle.transform.position;
                var distance = Vector3.Distance(currentPos, target);
                particle.transform.position = Vector3.MoveTowards(currentPos, target,_currentMoveSpeed * distance * Time.deltaTime);
            }
        }
        
        private void Active(ParticleSystemFacade nimbus)
        {
            _currentMoveSpeed = _speedRange.MinValue;
            _currentNimbus = nimbus;
            _currentNimbus.On();
            IsActive = true;
        }

        private IEnumerator ActiveRoutine(ParticleSystemFacade nimbus)
        {
            nimbus.TryGetAudioModule(out var audioModule);
            yield return new WaitUntil(() => audioModule.IsSleep());
            Active(nimbus);
        }
        
        private void Disable()
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