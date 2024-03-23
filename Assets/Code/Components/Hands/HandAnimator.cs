using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Hands
{
    public class HandAnimator : HandComponent, IGameInitListener
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        
        public void PlayEnter()
        {
            _spriteRenderer.enabled = true;
        }

        public void PlayExit()
        {
            _spriteRenderer.enabled = false;
        }

        public void GameInit()
        {
            PlayExit();
        }
    }
}