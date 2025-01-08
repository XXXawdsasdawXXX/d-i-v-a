using System;
using Code.Data;
using Code.Entities.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Diva
{
    public class DivaAnimationStateObserver : DivaComponent, IAnimationStateReader
    {
        public event Action<EDivaAnimationState> OnStateEntered;
        public event Action<EDivaAnimationState> OnStateExited;
        
        [field: SerializeField] public EDivaAnimationState State { get; private set; }

        private readonly int _transition_Seat_Hash = Animator.StringToHash("TransitionSeat");
        private readonly int _transition_Stand_Hash = Animator.StringToHash("TransitionStand");
        private readonly int _transition_Sleep_Hash = Animator.StringToHash("TransitionSleep");

        private readonly int _idle_Hash = Animator.StringToHash("Idle");
        private readonly int _eat_Hash = Animator.StringToHash("Eat");
        private readonly int _enter_Hash = Animator.StringToHash("Enter");
        private readonly int _exit_Hash = Animator.StringToHash("Exit");
        
        private readonly int _reaction_Voice_Hash = Animator.StringToHash("ReactionVoice");
        private readonly int _reaction_Mouse_Hash = Animator.StringToHash("ReactionMouse");
        
        private readonly int _hand_Show_Hash = Animator.StringToHash("HandShow");
        private readonly int _hand_Idle_Hash = Animator.StringToHash("HandIdle");
        private readonly int _hand_Hide_Hash = Animator.StringToHash("HandHide");
        
        
        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            
            OnStateEntered?.Invoke(State);
            
            Debugging.Log(this, $"Animation entered state: {State}", Debugging.Type.AnimationState);
        }

        public void ExitedState(int stateHash)
        {
            EDivaAnimationState state = StateFor(stateHash);
            
            OnStateExited?.Invoke(StateFor(stateHash));
            
            Debugging.Log(this, $"Animation exited state: {State}", Debugging.Type.AnimationState);
        }

        private EDivaAnimationState StateFor(int stateHash)
        {
            EDivaAnimationState state;
            
            //mode
            if (stateHash == _idle_Hash)
                state = EDivaAnimationState.Idle;
            else if (stateHash == _eat_Hash)
                state = EDivaAnimationState.Eat;
            else if (stateHash == _enter_Hash)
                state = EDivaAnimationState.Enter;
            else if (stateHash == _exit_Hash)
                state = EDivaAnimationState.Exit;

            //reactions
            else if (stateHash == _reaction_Voice_Hash)
                state = EDivaAnimationState.ReactionVoice;
            else if (stateHash == _reaction_Mouse_Hash)
                state = EDivaAnimationState.ReactionMouse;

            //hand
            else if (stateHash == _hand_Show_Hash)
                state = EDivaAnimationState.HandShow;
            else if (stateHash == _hand_Idle_Hash)
                state = EDivaAnimationState.HandIdle; 
            else if (stateHash == _hand_Hide_Hash)
                state = EDivaAnimationState.HandHide;
            
            //transitions
            else if (stateHash == _transition_Seat_Hash)
                state = EDivaAnimationState.TransitionSeat;
            else if (stateHash == _transition_Stand_Hash)
                state = EDivaAnimationState.TransitionStand;
            else if (stateHash == _transition_Sleep_Hash)
                state = EDivaAnimationState.TransitionSleep;
            else
                state = EDivaAnimationState.None;

            return state;
        }
    }
}