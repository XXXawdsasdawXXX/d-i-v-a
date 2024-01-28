using System.Collections;
using Code.Components.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Apples
{
    public class AppleBranchSpawner : MonoBehaviour, IService, IGameInitListener, IGameStartListener, IGameExitListener
    {
        private AppleConfig _appleConfig;
        private AppleBranch _appleBranch;
        private Apple _apple;

        private CharacterManager _characterManager;
        
        private Coroutine _coroutine;

        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _appleBranch = Container.Instance.FindEntity<AppleBranch>();
            _apple = Container.Instance.FindEntity<Apple>();

            _characterManager = Container.Instance.FindEntity<Characters.Character>().Manager;
            
            SubscribeToEvents(true);

            Debugging.Instance.Log($"Spawner init", Debugging.Type.Apple);
        }

        public void GameStart()
        {
            Spawn();
        }

        public void GameExit()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _apple.Event.EndLiveTimeEvent += OnAppleEnd;
            }
            else
            {
                _apple.Event.EndLiveTimeEvent -= OnAppleEnd;
            }
        }
        

        private void OnAppleEnd()
        {
            Spawn();
        }

        private void Spawn()
        {
            _coroutine ??= StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            var period = _appleConfig.SpawnCooldownMinutes.GetRandomValue() * 60;
            Debugging.Instance.Log($"Spawn routine -> period {period / 60} min", Debugging.Type.Apple);
            
            yield return new WaitForSeconds(period);

            _coroutine = null;
            
            if (_characterManager.GetAnimationMode() == CharacterAnimationMode.Sleep)
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