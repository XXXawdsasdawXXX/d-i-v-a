using Code.Components.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class AppleBranchController : MonoBehaviour, IService, IGameInitListener, IGameStartListener
    {
        private AppleConfig _appleConfig;
        private AppleBranch _appleBranch;
        private Apple _apple;

        private CharacterAnimationAnalytic _animationAnalytic;

        private TickCounter _tickCounter;

        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _appleBranch = Container.Instance.FindEntity<AppleBranch>();
            _apple = Container.Instance.FindEntity<Apple>();

            _animationAnalytic = Container.Instance.FindEntity<Characters.DIVA>()
                .FindCharacterComponent<CharacterAnimationAnalytic>();
            _tickCounter = new TickCounter();
            
            SubscribeToEvents(true);

            Debugging.Instance.Log($"Spawner init", Debugging.Type.Apple);
        }

        public void GameStart()
        {
            Spawn();
        }



        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _apple.Event.UseEvent +=  Spawn;
                _tickCounter.WaitedEvent += OnWaitedTickCounter;
            }
            else
            {
               // _apple.Event.EndLiveTimeEvent -= OnAppleEnd;
            }
        }
        
        private void Spawn()
        {
            var period = _appleConfig.SpawnCooldownTick.GetRandomValue();
            _tickCounter.StartWait(period);
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
    }
}