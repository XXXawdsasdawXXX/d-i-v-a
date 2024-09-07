using System;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Entities.Characters.AnimationReader.State
{
    public class CharacterAnimationStateObserver : CharacterComponent, IAnimationStateReader
    {
        [field: SerializeField] public CharacterAnimationState State { get; private set; }

        private readonly int _transition_Seat_Hash = Animator.StringToHash("TransitionSeat");
        private readonly int _transition_Stand_Hash = Animator.StringToHash("TransitionStand");
        private readonly int _transition_Sleep_Hash = Animator.StringToHash("TransitionSleep");

        private readonly int _idle_Hash = Animator.StringToHash("Idle");
        private readonly int _eat_Hash = Animator.StringToHash("Eat");
        private readonly int _enter_Hash = Animator.StringToHash("Enter");
        private readonly int _exit_Hash = Animator.StringToHash("Exit");
        private readonly int _reaction_Voice_Hash = Animator.StringToHash("ReactionVoice");
        private readonly int _reaction_Mouse_Hash = Animator.StringToHash("ReactionMouse");

        public event Action<CharacterAnimationState> OnStateEntered;
        public event Action<CharacterAnimationState> OnStateExited;


        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);
            OnStateEntered?.Invoke(State);
            Debugging.Instance?.Log($"Animation entered state: {State}", Debugging.Type.AnimationState);
        }

        public void ExitedState(int stateHash)
        {
            var state = StateFor(stateHash);
            OnStateExited?.Invoke(StateFor(stateHash));
            Debugging.Instance?.Log($"Animation exited state: {State}", Debugging.Type.AnimationState);
        }

        private CharacterAnimationState StateFor(int stateHash)
        {
            CharacterAnimationState state;


            //Mode
            if (stateHash == _idle_Hash)
                state = CharacterAnimationState.Idle;
            else if (stateHash == _eat_Hash)
                state = CharacterAnimationState.Eat;
            else if (stateHash == _enter_Hash)
                state = CharacterAnimationState.Enter;
            else if (stateHash == _exit_Hash)
                state = CharacterAnimationState.Exit;

            //Reactions
            else if (stateHash == _reaction_Voice_Hash)
                state = CharacterAnimationState.ReactionVoice;
            else if (stateHash == _reaction_Mouse_Hash)
                state = CharacterAnimationState.ReactionMouse;

            //Transitions
            else if (stateHash == _transition_Seat_Hash)
                state = CharacterAnimationState.TransitionSeat;
            else if (stateHash == _transition_Stand_Hash)
                state = CharacterAnimationState.TransitionStand;
            else if (stateHash == _transition_Sleep_Hash)
                state = CharacterAnimationState.TransitionSleep;
            else
                state = CharacterAnimationState.None;

            return state;
        }
    }
}