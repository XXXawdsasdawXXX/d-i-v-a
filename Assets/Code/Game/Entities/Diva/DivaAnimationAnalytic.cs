﻿using System;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Game.Entities.Diva
{
    public class DivaAnimationAnalytic : DivaComponent, ISubscriber
    {
        public event Action<EDivaAnimationMode> OnEnteredMode;
        public event Action<EDivaAnimationState> OnSwitchState;
        public event Action<EDivaAnimationState> OnStateExit;
        
        public EDivaAnimationMode CurrentMode { get; private set; }
        public EDivaAnimationState CurrentState { get; private set; }
        public bool IsTransition => _divaAnimationStateObserver != null && _divaAnimationStateObserver.State
            is EDivaAnimationState.TransitionSeat
            or EDivaAnimationState.TransitionSleep
            or EDivaAnimationState.TransitionStand;
        
        [SerializeField] private DivaAnimator _divaAnimator;
        [SerializeField] private DivaAnimationStateObserver _divaAnimationStateObserver;


        public void Subscribe()
        {
            _divaAnimator.OnModeEntered += _onEnteredModeEvent;
            _divaAnimationStateObserver.OnStateEntered += _onSwitchStateEvent;
            _divaAnimationStateObserver.OnStateExited += _onSwitchStateEvent;
        }
        
        public void Unsubscribe()
        {
            _divaAnimator.OnModeEntered -= _onEnteredModeEvent;
            _divaAnimationStateObserver.OnStateEntered -= _onSwitchStateEvent;
            _divaAnimationStateObserver.OnStateExited -= _onSwitchStateEvent;
        }

        public EDivaAnimationMode GetAnimationMode()
        {
            return _divaAnimator.Mode;
        }

        public EDivaAnimationState GetAnimationState()
        {
            return _divaAnimationStateObserver.State;
        }

        private void _onSwitchStateEvent(EDivaAnimationState state)
        {
            CurrentState = state;
            OnSwitchState?.Invoke(state);
        }

        private void _onEnteredModeEvent(EDivaAnimationMode mode)
        {
            CurrentMode = mode;
            OnEnteredMode?.Invoke(mode);
        }
    }
}