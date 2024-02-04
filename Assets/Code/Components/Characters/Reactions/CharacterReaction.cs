using System.Collections;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters.Reactions
{
    public abstract class CharacterReaction : MonoBehaviour, IGameExitListener
    {
        [SerializeField] private float _maxCooldownMinutes = 5;

        private bool _isReady;
        private Coroutine _cooldownCoroutine;

        public void GameExit()
        {
            if (_cooldownCoroutine != null)
            {
                StopCoroutine(_cooldownCoroutine);
            }
        }

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