using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Characters.Reactions
{
    public abstract class CharacterReaction : MonoBehaviour,  IGameInitListener
    {
        protected abstract int _cooldownTickCount { get; set; }

        private TickCounter _tickCounter;
        private bool _isReady = true;

        public void GameInit()
        {
            Init();
            SetCooldownMinutes();
            _tickCounter = new TickCounter(_cooldownTickCount);
            _tickCounter.WaitedEvent += () => _isReady = true;
        }

        protected virtual void InitReaction() { }

        protected abstract void SetCooldownMinutes();

        public bool IsReady()
        {
            return _tickCounter.IsExpectedStart && _isReady;
        }

       public virtual void StartReaction()
        {
            _isReady = false;
        }

        public virtual void StopReaction()
        {
            _tickCounter.StartWait();
        }

        protected virtual void Init(){}
    }
}