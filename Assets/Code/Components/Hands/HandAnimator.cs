using System;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Hands
{
    public class HandAnimator : HandComponent, IGameInitListener
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _handAnimator;

        private static readonly int HandEnter = Animator.StringToHash("HandEnter");

        private Action _endExitAnimationEvent;
        private Action _endEnterAnimationEvent;
        private static readonly int Exit = Animator.StringToHash("Exit");

        public void GameInit()
        {
            //PlayExitHand();
        }

        public void PlayEnterHand(Action onEndEnter = null)
        {
            // _spriteRenderer.enabled = true;
            _handAnimator.SetTrigger(HandEnter);
            _endEnterAnimationEvent = onEndEnter;
        }

        public void PlayExitHand(Action onEndExit = null)
        {
            //_spriteRenderer.enabled = false;
            _handAnimator.SetTrigger(Exit);
            _endExitAnimationEvent = onEndExit;
            Debugging.Instance.Log($"[animator] hide hand",Debugging.Type.Hand);
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