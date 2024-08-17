using System;
using Code.Components.Entities.Characters.AnimationReader.State;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class CharacterAnimationAnalytic : CharacterComponent, IGameInitListener, IGameExitListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;

        public CharacterAnimationMode CurrentMode { get; private set; }
        public CharacterAnimationState CurrentState { get; private set; }
        public bool IsTransition => _animationStateObserver != null && _animationStateObserver.State
            is CharacterAnimationState.TransitionSeat
            or CharacterAnimationState.TransitionSleep
            or CharacterAnimationState.TransitionStand;
        
        public event Action<CharacterAnimationMode> OnEnteredMode;
        public event Action<CharacterAnimationState> OnSwitchState;
        public event Action<CharacterAnimationState> OnStateExit;

        public void GameInit()
        {
            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public CharacterAnimationMode GetAnimationMode()
        {
            return _characterAnimator.Mode;
        }

        public CharacterAnimationState GetAnimationState()
        {
            return _animationStateObserver.State;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterAnimator.OnModeEntered += OnEnteredModeEvent;
                _animationStateObserver.OnStateEntered += OnSwitchStateEvent;
                _animationStateObserver.OnStateExited += OnSwitchStateEvent;
            }
            else
            {
                _characterAnimator.OnModeEntered -= OnEnteredModeEvent;
                _animationStateObserver.OnStateEntered -= OnSwitchStateEvent;
                _animationStateObserver.OnStateExited -= OnSwitchStateEvent;
            }
        }

        private void OnSwitchStateEvent(CharacterAnimationState state)
        {
            CurrentState = state;
            OnSwitchState?.Invoke(state);
        }

        private void OnEnteredModeEvent(CharacterAnimationMode mode)
        {
            CurrentMode = mode;
            OnEnteredMode?.Invoke(mode);
        }
    }
}