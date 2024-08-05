using System;
using Code.Components.Characters.AnimationReader.State;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters.Reactions
{
    public class CharacterAudioReaction : CharacterReaction, IGameExitListener
    {
        [Header("Components")] 
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterAnimationStateObserver _stateReader;
        protected override int _cooldownTickCount { get; set; }

        private LiveStateRangePercentageValue _effectAwakeningValue;
        private LiveStateStorage _liveStateStorage;

        public event Action EndReactionEvent;

        protected override void InitReaction()
        {
            SubscribeToEvents(true);
            base.InitReaction();
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        protected override void SetCooldownMinutes()
        {
            _cooldownTickCount = Container.Instance.FindConfig<TimeConfig>().Cooldown.ReactionMaxAudioClip;
            _effectAwakeningValue = Container.Instance.FindConfig<LiveStateConfig>().Awakening;
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        public override void StartReaction()
        {
            _characterAnimator.PlayReactionVoice();
            RemoveLiveStateValue();
            base.StartReaction();
            base.StopReaction();
        }

        private void RemoveLiveStateValue()
        {
            _liveStateStorage.AddPercentageValue(_effectAwakeningValue);
        }

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _stateReader.OnStateExited += OnAnimationOnStateExited;
            }
            else
            {
                _stateReader.OnStateExited -= OnAnimationOnStateExited;
            }
        }

        private void OnAnimationOnStateExited(CharacterAnimationState obj)
        {
            if (obj == CharacterAnimationState.ReactionVoice)
            {
                EndReactionEvent?.Invoke();
            }
        }

        #endregion
    }
}