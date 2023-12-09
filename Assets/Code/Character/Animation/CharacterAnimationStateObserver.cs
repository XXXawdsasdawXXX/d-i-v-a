using System;
using UnityEngine;

namespace Code.Character
{
    public class CharacterAnimationStateObserver :MonoBehaviour, IAnimationStateReader
    {
        private readonly int _transition_StandToSeat_Hash = Animator.StringToHash("Transition_StandToSeat");
        private readonly int _seatIdle_Hash = Animator.StringToHash("Seat_Idle");
        private readonly int _Seat_Reaction_Voice_Hash = Animator.StringToHash("Seat_Reaction_Voice");
        
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
            
            if (stateHash == _Seat_Reaction_Voice_Hash)
            {
                state = CharacterAnimatorState.ReactionVoice;
            }
            else if (stateHash == _transition_StandToSeat_Hash)
            {
                state = CharacterAnimatorState.Transition;
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