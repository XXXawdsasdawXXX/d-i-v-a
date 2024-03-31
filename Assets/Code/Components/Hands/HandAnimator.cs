using System;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Hands
{
    public class HandAnimator : HandComponent, IGameInitListener
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Action _endEnterAnimationEvent;
        
        public void PlayEnter(Action onEndEnter = null)
        {
            _spriteRenderer.enabled = true;
            _endEnterAnimationEvent = onEndEnter;
        }

        public void PlayExit()
        {
            _spriteRenderer.enabled = false;
        }

        public void GameInit()
        {
            PlayExit();
        }

        #region Aimation events

        private void InvokeEndEnterEvent()
        {
            _endEnterAnimationEvent?.Invoke();
        }

        #endregion
    }
}