using System;
using UnityEngine;

namespace Code.Components.Entities
{
    public class HandAnimator : HandComponent
    {
        [SerializeField] private Animator _handAnimator;

        private static readonly int Enter = Animator.StringToHash("HandEnter");
        private static readonly int Exit = Animator.StringToHash("Exit");

        private Action _endExitAnimationEvent;
        private Action _endEnterAnimationEvent;

        [ContextMenu("Enter")]
        public void PlayEnterHand(Action onEndEnter = null)
        {
            _handAnimator.ResetTrigger(Exit);
            _handAnimator.SetTrigger(Enter);
            _endEnterAnimationEvent = onEndEnter;
        }

        [ContextMenu("Exit")]
        public void PlayExitHand(Action onEndExit = null)
        {
            _handAnimator.ResetTrigger(Enter);
            _handAnimator.SetTrigger(Exit);
            _endExitAnimationEvent = onEndExit;
        }

        #region Aimation events

        /// <summary>
        /// Animation event
        /// </summary>
        private void InvokeEndEnterEvent()
        {
            _endEnterAnimationEvent?.Invoke();
        }

        /// <summary>
        /// Animation event
        /// </summary>
        private void InvokeEndExitEvent()
        {
            _endExitAnimationEvent?.Invoke();
        }

        #endregion
    }
}