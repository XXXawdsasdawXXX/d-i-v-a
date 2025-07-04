﻿using System;
using System.Collections;
using Code.Data;
using UnityEngine;

namespace Code.Game.Entities.Common
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
            WaitForSeconds period = new(_frameDelay);
            for (int i = 0; i < _sprites.Length - 1; i++)
            {
                _spriteMask.sprite = _sprites[i];
                yield return period;
            }

            OnShown?.Invoke();
            yield return period;
            Disable();
        }

        public void Active(Action OnTurnedOn = null)
        {
            _coroutine = StartCoroutine(ShowAnimation(OnTurnedOn));
        }

        public void Disable(Action onTurnedOff = null)
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