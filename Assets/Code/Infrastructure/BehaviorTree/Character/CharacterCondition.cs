using Code.Components.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Character
{
    public class CharacterCondition: IService, IGameInitListener
    {
        [Header("D I V A")]
        private CharacterLiveStatesAnalytic _statesAnalytic;
        private CharacterLiveState _sleepState;
        private CharacterAnimationAnalytic _animationAnalytic;
        
        [Header("Services")]
        private TimeObserver _timeObserver;
        private InteractionStorage _interactionStorage;
      
        [Header("Static values")]
        private float _sleepHealValue;
        private int _stoppingTicksToMaximumSleepValues;
        private LiveStateStorage _liveStateStorage;

        public void GameInit()
        {
            //d i v a---------------------------------------------------------------------------------------------------
            var diva = Container.Instance.FindEntity<DIVA>();
            _statesAnalytic = diva.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _animationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
            
            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
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
                    Debugging.Instance.ErrorLog(this,"не нашел стейт сна");
                }
            };
        }

        #region Behavior tree
        
        public bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out var key, out var statePercent)
                   && key is LiveStateKey.Trust or LiveStateKey.Hunger
                   && statePercent < 0.4f;
        }
        
        public bool IsCanSleep(float bonusMinPercent = 0)
        {
            var minPercent = 0.3f + bonusMinPercent; 
            Debugging.Instance.Log($"Проверка на сон:" +
                                   $" {_sleepState != null}" +
                                   $" && ({_timeObserver.IsNightTime()}||{_sleepState?.GetPercent() < minPercent})" +
                                   $" && {_sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max}",
                Debugging.Type.CharacterCondition);
            
            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState.GetPercent() < minPercent) && 
                   _sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max;
            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState?.GetPercent() < minPercent);
        }

        public bool IsCanExitWhenSleep()
        {
            _statesAnalytic.TryGetLowerSate(out LiveStateKey lowerKey, out var lowerStatePercent);
           
            var randomResult = Random.Range(0, 100) >= 50;
           
            Debugging.Instance.Log($"Проверка на выход во сне:" +
                                   $" {lowerKey is LiveStateKey.Trust}" +
                                   $" && ({lowerStatePercent <= 0.4f})" +
                                   $" && {randomResult}",
                Debugging.Type.CharacterCondition);
            
            return lowerKey is LiveStateKey.Trust && lowerStatePercent <= 0.4f && randomResult;
        }

        public bool IsCanStand()
        {
            return true;
        }
        
        #endregion

        #region VFX
        public bool CanShowNimbus()
        {
            var isCorrectState = _animationAnalytic.GetAnimationState()
                is not CharacterAnimationState.Enter
                or CharacterAnimationState.Exit
                or CharacterAnimationState.TransitionSeat
                or CharacterAnimationState.TransitionSleep
                or CharacterAnimationState.TransitionStand;

            var isCorrectMode = _animationAnalytic.GetAnimationMode()
                is not CharacterAnimationMode.Sleep;

            var isCorrectInteractionResult = _interactionStorage.GetDominantInteractionType()
                is not InteractionType.Normal;

            Debugging.Instance.Log($"Проверка на нимбус:" +
                                   $" {!_animationAnalytic.IsTransition}" +
                                   $" && {isCorrectInteractionResult}" +
                                   $" && {isCorrectMode}" +
                                   $" && {isCorrectState}",
                Debugging.Type.CharacterCondition);
            
            return !_animationAnalytic.IsTransition && isCorrectInteractionResult && isCorrectMode && isCorrectState;
        }
        
        #endregion
    }
}
