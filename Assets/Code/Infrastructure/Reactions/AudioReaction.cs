﻿using System;
using Code.Components.Entities;
using Code.Components.Entities.AnimationReader.State;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Reactions
{
    public class AudioReaction : Reaction, IGameExitListener
    {
        [Header("Components")]
        private DivaAnimator _divaAnimator;
        private DivaAnimationStateObserver _stateReader;
        
        [Header("Services")]
        private LiveStateStorage _liveStateStorage;
        
        [Header("Values")]
        private LiveStateRangePercentageValue _effectAwakeningValue;
        
        public event Action EndReactionEvent;

        protected override void Init()
        {
            Diva diva = Container.Instance.FindEntity<Diva>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _stateReader = diva.FindCharacterComponent<DivaAnimationStateObserver>();
            
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _effectAwakeningValue = Container.Instance.FindConfig<LiveStateConfig>().Awakening;
           
            SubscribeToEvents(true);
            base.Init();
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.MaxAudioClipReactionMin;
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public override void StartReaction()
        {
            _divaAnimator.PlayReactionVoice();
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