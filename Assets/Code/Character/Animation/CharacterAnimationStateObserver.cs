using System;
using UnityEngine;

namespace Code.Character
{
    public class CharacterAnimationStateObserver :MonoBehaviour, IAnimationStateReader
    {
        private readonly int _transition_Seat_Hash = Animator.StringToHash("TransitionSeat");
        private readonly int _transition_Stand_Hash = Animator.StringToHash("TransitionStand");
        private readonly int _seatIdle_Hash = Animator.StringToHash("Idle");
        private readonly int _reaction_Voice_Hash = Animator.StringToHash("ReactionVoice");
        
        public event Action<CharacterAnimatorState> StateEnteredEvent;
        public event Action<CharacterAnimatorState> StateExitedEvent;
        public CharacterAnimatorState State { get; private set; }
        
        
        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            StateEnteredEvent?.Invoke(State);
        }
        
        public void ExitedState(int stateHash)
        {
            StateExitedEvent?.Invoke(StateFor(stateHash));
        }
        
        private CharacterAnimatorState StateFor(int stateHash)
        {
            CharacterAnimatorState state;
            
            if (stateHash == _reaction_Voice_Hash)
            {
                state = CharacterAnimatorState.ReactionVoice;
            }
            else if (stateHash == _transition_Seat_Hash)
            {
                state = CharacterAnimatorState.TransitionSeat;
            }
            else if (stateHash == _transition_Stand_Hash)
            {
                state = CharacterAnimatorState.TransitionStand;
            }
            else if(stateHash == _seatIdle_Hash)
            {
                state = CharacterAnimatorState.Idle;
            }
            else
            {
                state = CharacterAnimatorState.None;
            }

            Debug.Log($"State {state}");
            return state;
        }
    }
}