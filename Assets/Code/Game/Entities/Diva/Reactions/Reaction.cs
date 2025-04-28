using Code.Game.Services;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.Entities.Diva.Reactions
{
    [Preserve]
    public abstract class Reaction :  IInitializeListener
    {
        private TickCounter _tickCounter;
        private int _cooldownTickCount;
        private bool _isReady = true;

        public async UniTask GameInitialize()
        {
            await InitializeReaction();
            
            GetCooldownMinutes();
         
            _tickCounter = new TickCounter(_cooldownTickCount, isLoop: false);
         
            _tickCounter.OnWaitIsOver += () =>
            {
                _isReady = true;
            };
        }

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

        protected virtual UniTask InitializeReaction()
        {
            return UniTask.CompletedTask;
        }

        protected abstract int GetCooldownMinutes();
    }
}