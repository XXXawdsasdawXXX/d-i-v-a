using System;
using Code.Components.Entities.AnimationReader.State;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Entities
{
    public class DivaAnimationAnalytic : DivaComponent, IGameInitListener, IGameExitListener
    {
        [SerializeField] private DivaAnimator _divaAnimator;
        [SerializeField] private DivaAnimationStateObserver _animationStateObserver;

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
            return _divaAnimator.Mode;
        }

        public CharacterAnimationState GetAnimationState()
        {
            return _animationStateObserver.State;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _divaAnimator.OnModeEntered += OnEnteredModeEvent;
                _animationStateObserver.OnStateEntered += OnSwitchStateEvent;
                _animationStateObserver.OnStateExited += OnSwitchStateEvent;
            }
            else
            {
                _divaAnimator.OnModeEntered -= OnEnteredModeEvent;
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