using System;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Entities.Hands
{
    public class HandAnimator : HandComponent
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _handAnimator;

        private static readonly int Enter = Animator.StringToHash("HandEnter");
        private static readonly int Exit = Animator.StringToHash("Exit");

        private Action _endExitAnimationEvent;
        private Action _endEnterAnimationEvent;

        [ContextMenu("Enter")]
        public void PlayEnterHand(Action onEndEnter = null)
        {
            // _spriteRenderer.enabled = true;
            _handAnimator.ResetTrigger(Exit);
            _handAnimator.SetTrigger(Enter);
            _endEnterAnimationEvent = onEndEnter;
        }

        [ContextMenu("Exit")]
        public void PlayExitHand(Action onEndExit = null)
        {
            //_spriteRenderer.enabled = false;
            _handAnimator.ResetTrigger(Enter);
            _handAnimator.SetTrigger(Exit);
            _endExitAnimationEvent = onEndExit;
            Debugging.Instance.Log($"[animator] hide hand", Debugging.Type.Hand);
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