using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Character
{
    public class CharacterCondition: IService, IGameInitListener, IGameLoadListener
    {
        [Header("Character")]
        private CharacterLiveStatesAnalytic _statesAnalytic;
        [Header("Services")]
        private TimeObserver _timeObserver;
        private CharacterLiveState _sleepState;
        [Header("Static values")]
        private float _sleepHealValue;
        private int _stoppingTicksToMaximumSleepValues;
        private LiveStateStorage _liveStateStorage;

        public void GameInit()
        {
            //character-------------------------------------------------------------------------------------------------
            var character = Container.Instance.FindEntity<DIVA>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            //static values---------------------------------------------------------------------------------------------
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            var liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            _sleepHealValue = liveStateConfig.GetStaticParam(LiveStateKey.Sleep).HealValue;
            var timeConfig = Container.Instance.FindConfig<TimeConfig>();
            _stoppingTicksToMaximumSleepValues = timeConfig.Duration.StoppingTicksToMaximumSleepValues;
        }
            
        public void GameLoad()
        {
            if (!_liveStateStorage.TryGetLiveState(LiveStateKey.Sleep, out _sleepState))
            {
                Debugging.Instance.ErrorLog("[CharacterCondition] не нашел стейт сна");
            }
        }

        public bool IsCanSleep(float sleepStatePercent = 0.5f)
        {
            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState.GetPercent() < sleepStatePercent) && 
                   _sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max;
        }

        public bool IsCanExitWhenSleep()
        {
            _statesAnalytic.TryGetLowerSate(out LiveStateKey lowerKey, out var lowerStatePercent);
            return lowerKey is LiveStateKey.Trust && lowerStatePercent <= 0.4f && Random.Range(0, 100) >= 50;
        }
    }
}