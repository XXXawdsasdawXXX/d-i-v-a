using System;
using Code.Data;
using Code.Entities.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Hand
{
    public class HandAnimationStateObserver : HandComponent, IAnimationStateReader
    {
        public event Action<EHandAnimationMode> OnStateEntered;
        public event Action<EHandAnimationMode> OnStateExited;

        [field: SerializeField] public EHandAnimationMode State { get; private set; }

        private readonly int _idle_Hash = Animator.StringToHash("Idle");
        private readonly int _enter_Hash = Animator.StringToHash("Enter");
        private readonly int _exit_Hash = Animator.StringToHash("Exit");
        
        
        public void EnteredState(int stateHash)
        {
            State = StateFor(stateHash);

            OnStateEntered?.Invoke(State);

#if DEBUGGING
            Debugging.Log(this, $"[EnteredState] {State}.", Debugging.Type.Hand);
#endif
        }

        public void ExitedState(int stateHash)
        {
            EHandAnimationMode state = StateFor(stateHash);

            OnStateExited?.Invoke(StateFor(stateHash));

#if DEBUGGING
            Debugging.Log(this, $"[ExitedState] {State}.", Debugging.Type.Hand);
#endif
        }

        private EHandAnimationMode StateFor(int stateHash)
        {
            EHandAnimationMode state;
            
            if (stateHash == _idle_Hash)
                state = EHandAnimationMode.Idle;
            else if (stateHash == _enter_Hash)
                state = EHandAnimationMode.Enter;
            else if (stateHash == _exit_Hash)
                state = EHandAnimationMode.Exit;
            else
                state = EHandAnimationMode.None;
            
            return state;
        }
    }
}