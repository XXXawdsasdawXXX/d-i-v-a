using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;

namespace Code.Components.Entities.Characters.Reactions
{
    public abstract class Reaction :  IGameInitListener
    {
        private int _cooldownTickCount;

        private TickCounter _tickCounter;
        private bool _isReady = true;

        public void GameInit()
        {
            Init();
            GetCooldownMinutes();
            _tickCounter = new TickCounter(_cooldownTickCount, isLoop: false);
            _tickCounter.OnWaitIsOver += () => _isReady = true;
        }
        
        protected abstract int GetCooldownMinutes();

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

        protected virtual void Init()
        {
        }
    }
}