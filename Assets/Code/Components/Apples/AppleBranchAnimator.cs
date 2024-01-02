using System;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleBranchAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int _activeHash = Animator.StringToHash("Active");
        
        private event Action EndEnterEvent;
        private event Action EndExitEvent;
        
        public void PlayEnter(Action onEndAnimation = null)
        {
            _animator.SetBool(_activeHash, true);
            EndEnterEvent = onEndAnimation;
        }

        public void PlayExit(Action onEndAnimation = null)
        {
            _animator.SetBool(_activeHash, false);
            EndExitEvent = onEndAnimation;
        }

        private void InvokeEnterEnd()
        {
            EndEnterEvent?.Invoke();
        }

        private void InvokeExitEnd()
        {
            EndExitEvent?.Invoke();
        }
    }
}