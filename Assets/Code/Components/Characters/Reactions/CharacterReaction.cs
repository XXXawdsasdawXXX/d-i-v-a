using System.Collections;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters.Reactions
{
    public abstract class CharacterReaction : MonoBehaviour, IGameExitListener, IGameInitListener
    {
        protected abstract float _maxCooldownMinutes { get; set; }

        private bool _isReady = true;
        private Coroutine _cooldownCoroutine;

        public void GameInit()
        {
            SetCooldownMinutes();
        }

        public void GameExit()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }
        }

        protected abstract void SetCooldownMinutes();

        public bool IsReady()
        {
            return _isReady;
        }

       public virtual void StartReaction()
        {
            _isReady = false;
        }

        public virtual void StopReaction()
        {
            RefreshCooldown();
        }

        private void RefreshCooldown()
        {
            _cooldownCoroutine = StartCoroutine(RefreshCooldownRoutine());
        }

        private IEnumerator RefreshCooldownRoutine()
        {
            yield return new WaitForSeconds(_maxCooldownMinutes * 60);
            _isReady = true;
        }
    }
}