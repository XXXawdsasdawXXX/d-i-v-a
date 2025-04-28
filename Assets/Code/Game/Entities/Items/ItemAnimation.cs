using System;
using UnityEngine;

namespace Code.Game.Entities.Items
{
    public class ItemAnimation: MonoBehaviour
    {
        [SerializeField] private Animator _animator;
     
        private static readonly int Enter_t = Animator.StringToHash("Enter");
        private static readonly int Destroy_t = Animator.StringToHash("Destroy");
        private static readonly int Use_t = Animator.StringToHash("Use");

        public event Action OnPlayedEnter;
        public event Action OnPlayedDestroy;
        public event Action OnPlayedUse;

        public void SetController(RuntimeAnimatorController controller)
        {
            _animator.runtimeAnimatorController = controller;
        }
        
        public void PlayEnter(Action onPlayed = null)
        {
            _animator.SetTrigger(Enter_t);
            OnPlayedEnter = onPlayed;
        }

        public void PlayUse(Action onPlayed = null)
        {
            _animator.SetTrigger(Use_t);
            OnPlayedUse = onPlayed;
        }
        
        public void PlayDestroy(Action onPlayed = null)
        {
            _animator.SetTrigger(Destroy_t);
            OnPlayedDestroy = onPlayed;
        }

        #region Animation Events

        /// <summary>
        /// Animation event
        /// </summary>
        private void InvokeEndEnterEvent()
        {
            OnPlayedEnter?.Invoke();
        }
        
        /// <summary>
        /// Animation event
        /// </summary>
        private void InvokeEndUseEvent()
        {
            OnPlayedUse?.Invoke();
        }

        /// <summary>
        /// Animation event
        /// </summary>
        private void InvokeEndDestroyEvent()
        {
            OnPlayedDestroy?.Invoke();
        }
        
        #endregion

    }
}