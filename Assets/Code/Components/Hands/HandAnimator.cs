using UnityEngine;

namespace Code.Components.Hands
{
    public class HandAnimator : MonoBehaviour
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
    }
}