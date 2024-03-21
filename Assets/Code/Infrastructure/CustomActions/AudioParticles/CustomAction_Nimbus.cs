using System.Collections;
using System.Linq;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Nimbus : CustomAction_AudioParticle, IGameExitListener
    {
        private CharacterAnimationAnalytic _animationAnalytic;
        private CoroutineRunner _coroutineRunner;
        private InteractionStorage _interactionStorage;

        private ParticleSystemFacade _currentNimbus;

        protected override void Init()
        {
            _animationAnalytic = _diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            SubscribeToEvents(true);
            base.Init();
        }

        public void GameExit()
        {
            if (!_isUsed)
            {
                return;
            }
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

        protected override void StartAction()
        {
            if (_animationAnalytic.GetAnimationMode() == CharacterAnimationMode.Sleep)
            {
                DisableCurrentNimbus();
                return;
            }
            _coroutineRunner.StartRoutine(AwaitStart());
        }

        private IEnumerator AwaitStart()
        {
            Debugging.Instance.Log("[nimbus] await PLAY ", Debugging.Type.CustomAction);
            yield return new WaitUntil(() => _animationAnalytic.GetCharacterAnimationState() != CharacterAnimationState.Enter);
            Debugging.Instance.Log("[nimbus] PLAY ", Debugging.Type.CustomAction);
            IsActive = true;
            var dominantInteractionType = _interactionStorage.GetDominantInteractionType();
            if (dominantInteractionType == InteractionType.Good)
            {
                Show(ParticleType.Nimbus_light);
            }
            else if(dominantInteractionType == InteractionType.Bad)
            {
                Show(ParticleType.Nimbus_dark);
            }
            else
            {
                DisableCurrentNimbus();
            }
        }

        protected override void UpdateParticles()
        {
            if (!_isUsed || _particlesSystems == null)
            {
                return;
            }

            foreach (var particlesSystem in _particlesSystems)
            {
                particlesSystem.transform.position = GetParticlePosition();
            }
        }

        private void Show(ParticleType particleType)
        {
            if (_currentNimbus != null && _currentNimbus.Type != particleType)
            {
                DisableCurrentNimbus();
            }

            var nimbus_light = _particlesSystems.FirstOrDefault(p => p.Type ==particleType);

            if (nimbus_light != null)
            {
                ActiveNimbus(nimbus_light);
            }
        }

        private void ActiveNimbus(ParticleSystemFacade nimbus)
        {
            _currentNimbus = nimbus;
            _currentNimbus.On();
        }

        private void DisableCurrentNimbus()
        {
            _currentNimbus?.Off();
            _currentNimbus = null;
        }

        private Vector3 GetParticlePosition()
        {
            return _characterModeAdapter.GetWorldHeatPoint() + Vector3.up * 0.2f;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _interactionStorage.SwitchDominationTypeEvent += OnSwitchInteractionDominationType;
                _animationAnalytic.SwitchModeEvent += OnSwitchAnimationMode;
            }
            else
            {
                _interactionStorage.SwitchDominationTypeEvent -= OnSwitchInteractionDominationType;
                _animationAnalytic.SwitchModeEvent -= OnSwitchAnimationMode;
            }
        }

        private void OnSwitchAnimationMode(CharacterAnimationMode mode)
        {
            StartAction();
        }

        private void OnSwitchInteractionDominationType(InteractionType currentType)
        {
            StartAction();
        }
    }
}