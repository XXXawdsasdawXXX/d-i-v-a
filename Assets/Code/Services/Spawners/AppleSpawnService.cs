using System.Collections;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace Code.Components.Objects
{
    public class AppleSpawnService : SpawnService<AppleBranch>, IService, IGameInitListener
    {
        private AppleConfig _appleConfig;
        private AppleBranch _appleBranch;
        private CoroutineRunner _coroutineRunner;

        public void GameInit()
        {
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();
            _appleBranch = Container.Instance.FindEntity<AppleBranch>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
        }

        protected override void Spawn()
        {
            _coroutineRunner.StartRoutine(SpawnRoutine());
        }

        protected override void DeSpawn()
        {
            _coroutineRunner.StopCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(_appleConfig.SpawnCooldownMinutes.GetRandomValue() * 60);
            _appleBranch.GrowBranch();
            _coroutineRunner.StartRoutine(SpawnRoutine());
        }
    }
}