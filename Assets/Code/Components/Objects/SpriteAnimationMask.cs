using System;
using System.Collections;
using Code.Data.Interfaces;
using UnityEngine;

namespace Code.Components.Objects
{
    public class SpriteAnimationMask : MonoBehaviour, IActivated
    {
        [SerializeField] private SpriteMask _spriteMask;
        [SerializeField] private float _frameDelay = 0.10f;
        [SerializeField] private Sprite[] _sprites;
        
        private Coroutine _coroutine;

        private void OnDestroy()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        public void Activate(Action OnShown)
        {
            _coroutine = StartCoroutine(ShowAnimation(OnShown));
        }

        public void Activate()
        {
            _coroutine = StartCoroutine(ShowAnimation());
        }

        public void Deactivate()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _spriteMask.enabled = false;
            _spriteMask.sprite = _sprites[0];
        }

        private IEnumerator ShowAnimation(Action OnShown = null)
        {
            _spriteMask.enabled = true;
            var period = new WaitForSeconds(_frameDelay);
            for (int i = 0; i < _sprites.Length; i++)
            {
                _spriteMask.sprite = _sprites[i];
                yield return period;
            }
            OnShown?.Invoke();
            Deactivate();
        }
        
    }
}