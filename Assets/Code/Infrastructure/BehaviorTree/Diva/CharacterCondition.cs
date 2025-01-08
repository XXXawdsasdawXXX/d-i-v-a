using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public class CharacterCondition : IService, IGameInitListener
    {
        [Header("D I V A")]
        private DivaLiveStatesAnalytic _statesAnalytic;
        private CharacterLiveState _sleepState;
        private DivaAnimationAnalytic _animationAnalytic;

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
            Entities.Diva.DivaEntity diva = Container.Instance.FindEntity<Entities.Diva.DivaEntity>();
            _statesAnalytic = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();

            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            //static values---------------------------------------------------------------------------------------------
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            LiveStateConfig liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            _sleepHealValue = liveStateConfig.GetStaticParam(ELiveStateKey.Sleep).HealValue;
            TimeConfig timeConfig = Container.Instance.FindConfig<TimeConfig>();
            _stoppingTicksToMaximumSleepValues = timeConfig.Duration.StoppingTicksToMaximumSleepValues;

            _liveStateStorage.OnInit += () =>
            {
                if (!_liveStateStorage.TryGetLiveState(ELiveStateKey.Sleep, out _sleepState))
                {
                    Debugging.LogError(this, "не нашел стейт сна");
                }
            };
        }

        #region Behavior tree

        public bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out ELiveStateKey key, out float statePercent)
                   && key is ELiveStateKey.Trust or ELiveStateKey.Hunger
                   && statePercent < 0.4f;
        }

        public bool IsCanSleep(float bonusMinPercent = 0)
        {
            float minPercent = 0.3f + bonusMinPercent;
            Debugging.Log($"Проверка на сон:" +
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
            _statesAnalytic.TryGetLowerSate(out ELiveStateKey lowerKey, out float lowerStatePercent);

            bool randomResult = Random.Range(0, 100) >= 50;

            Debugging.Log($"Проверка на выход во сне:" +
                          $" {lowerKey is ELiveStateKey.Trust}" +
                          $" && ({lowerStatePercent <= 0.4f})" +
                          $" && {randomResult}",
                Debugging.Type.CharacterCondition);

            return lowerKey is ELiveStateKey.Trust && lowerStatePercent <= 0.4f && randomResult;
        }

        public bool IsCanStand()
        {
            return true;
        }

        #endregion

        #region VFX

        public bool CanShowNimbus()
        {
            bool isCorrectState = _animationAnalytic.GetAnimationState()
                is not EDivaAnimationState.Enter
                or EDivaAnimationState.Exit
                or EDivaAnimationState.TransitionSeat
                or EDivaAnimationState.TransitionSleep
                or EDivaAnimationState.TransitionStand;

            bool isCorrectMode = _animationAnalytic.GetAnimationMode()
                is not EDivaAnimationMode.Sleep;

            bool isCorrectInteractionResult = _interactionStorage.GetDominantInteractionType()
                is not EInteractionType.Normal;

            Debugging.Log($"Проверка на нимбус:" +
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