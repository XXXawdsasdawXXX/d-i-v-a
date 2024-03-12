
using System.Collections;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Nimbus : CustomAction_AudioParticle
    {
        private CharacterAnimationAnalytic _animationAnalytic;
        private CoroutineRunner _coroutineRunner;

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Nimbus;
        }

        protected override void Init()
        {
            _animationAnalytic = _diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            base.Init();
        }

        protected override ParticleType GetParticleType()
        {
            return ParticleType.Nimbus;
        }

        protected override void StartAction()
        {
            _coroutineRunner.StartRoutine(AwaitStart());
        }

        private IEnumerator AwaitStart()
        {
            yield return new WaitUntil(() =>  _animationAnalytic.GetCharacterAnimationState() != CharacterAnimationState.Enter);
            Debugging.Instance.Log("PLAY");
            base.StartAction();
        }
        protected override void UpdateParticles()
        {
            if (_isNotUsed || _particlesSystems == null)
            {
                return;
            }

            foreach (var particle in _particlesSystems)
            {
                if (!particle.IsPlay) continue;
                particle.transform.position = GetParticlePosition();
            }
        }

        private Vector3 GetParticlePosition()
        {
            return _characterModeAdapter.GetWorldEatPoint() + Vector3.up * 0.5f;
        }
    }
}