using Code.Components.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class AppleBranchController : MonoBehaviour, IService, IGameInitListener,IGameExitListener, IProgressWriter
    {
        private AppleConfig _appleConfig;
        private AppleBranch _appleBranch;
        private Apple _apple;

        private CharacterAnimationAnalytic _animationAnalytic;

        private TickCounter _tickCounter;

        #region Live cycle
        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _appleBranch = Container.Instance.FindEntity<AppleBranch>();
            _apple = Container.Instance.FindEntity<Apple>();

            _animationAnalytic = Container.Instance.FindEntity<DIVA>().FindCharacterComponent<CharacterAnimationAnalytic>();
            _tickCounter = new TickCounter(isLoop: false);
            
            SubscribeToEvents(true);

            Debugging.Instance.Log($"Spawner init", Debugging.Type.Apple);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            var savedTicks = playerProgress.Cooldowns.AppleRemainingTick;
            _tickCounter.StartWait(savedTicks > 0 ? savedTicks : GetRandomCooldownTicks());
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.Cooldowns.AppleRemainingTick = _tickCounter.GetRemainingTick();
        }

        #endregion

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _apple.Event.UseEvent +=  Spawn;
                _tickCounter.WaitedEvent += OnWaitedTickCounter;
            }
            else
            {
                _apple.Event.UseEvent -=  Spawn;
                _tickCounter.WaitedEvent -= OnWaitedTickCounter;
            }
        }

        private void Spawn()
        {
            _tickCounter.StartWait(GetRandomCooldownTicks());
        }

        private void OnWaitedTickCounter()
        {
            if (_animationAnalytic.GetAnimationMode() == CharacterAnimationMode.Sleep)
            {
                Spawn();
            }
            else
            {
                _appleBranch.GrowBranch();
            }
        }

        #endregion

        #region Methods

        private int GetRandomCooldownTicks()
        {
            return _appleConfig.SpawnCooldownTick.GetRandomValue();
        }

        #endregion
    }
}