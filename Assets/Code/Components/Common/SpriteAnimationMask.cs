using System;
using System.Collections;
using Code.Data.Interfaces;
using UnityEngine;

namespace Code.Components.Common
{
    public class SpriteAnimationMask : CommonComponent, IToggle
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

        private IEnumerator ShowAnimation(Action OnShown = null)
        {
            _spriteMask.enabled = true;
            WaitForSeconds period = new WaitForSeconds(_frameDelay);
            for (int i = 0; i < _sprites.Length - 1; i++)
            {
                _spriteMask.sprite = _sprites[i];
                yield return period;
            }

            OnShown?.Invoke();
            yield return period;
            Off();
        }

        public void On(Action OnTurnedOn = null)
        {
            _coroutine = StartCoroutine(ShowAnimation(OnTurnedOn));
        }

        public void Off(Action onTurnedOff = null)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _spriteMask.enabled = false;
            _spriteMask.sprite = _sprites[0];
            onTurnedOff?.Invoke();
        }
    }
}