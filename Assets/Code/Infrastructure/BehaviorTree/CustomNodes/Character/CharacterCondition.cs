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
    public class CharacterCondition: IService, IGameInitListener
    {
        [Header("Character")]
        private CharacterLiveStatesAnalytic _statesAnalytic;
        private CharacterLiveState _sleepState;
        
        [Header("Services")]
        private TimeObserver _timeObserver;
      
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

            _liveStateStorage.OnInit += () =>
            {
                if (!_liveStateStorage.TryGetLiveState(LiveStateKey.Sleep, out _sleepState))
                {
                    Debugging.Instance.ErrorLog("[CharacterCondition] не нашел стейт сна");
                }
            };
        }
        
        public bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out var key, out var statePercent)
                   && key is LiveStateKey.Trust or LiveStateKey.Hunger
                   && statePercent < 0.4f;
        }
        
        public bool IsCanSleep(float sleepStatePercent = 0.5f)
        {
            Debugging.Instance.Log($"Проверка на сон:" +
                                   $" {_sleepState != null}" +
                                   $" && ({_timeObserver.IsNightTime()}||{_sleepState?.GetPercent() < sleepStatePercent})" +
                                   $" && {_sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max}",
                Debugging.Type.BehaviorTree);
            
            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState.GetPercent() < sleepStatePercent) && 
                   _sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max;
            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState?.GetPercent() < sleepStatePercent);
        }

        public bool IsCanExitWhenSleep()
        {
            _statesAnalytic.TryGetLowerSate(out LiveStateKey lowerKey, out var lowerStatePercent);
           
            var randomResult = Random.Range(0, 100) >= 50;
           
            Debugging.Instance.Log($"Проверка на выход во сне:" +
                                   $" {lowerKey is LiveStateKey.Trust}" +
                                   $" && ({lowerStatePercent <= 0.4f})" +
                                   $" && {randomResult}",
                Debugging.Type.BehaviorTree);
            
            return lowerKey is LiveStateKey.Trust && lowerStatePercent <= 0.4f && randomResult;
        }

        public bool IsCanStand()
        {
            return true;
        }
    }
}