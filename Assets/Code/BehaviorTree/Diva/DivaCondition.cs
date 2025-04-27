using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.BehaviorTree.Diva
{
    [Preserve]
    public class DivaCondition : IService, IInitializeListener, IStartListener
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

        public UniTask GameInitialize()
        {
            //d i v a---------------------------------------------------------------------------------------------------
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _statesAnalytic = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();

            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            //static values---------------------------------------------------------------------------------------------
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            
            LiveStateConfig liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            _sleepHealValue = liveStateConfig.GetStaticParam(ELiveStateKey.Sleep).HealValue;
          
            TimeConfig timeConfig = Container.Instance.FindConfig<TimeConfig>();
            _stoppingTicksToMaximumSleepValues = timeConfig.Duration.StoppingTicksToMaximumSleepValues;
            
            return UniTask.CompletedTask;
        }

        #region Behavior tree

        public UniTask GameStart()
        {
            _liveStateStorage.TryGetLiveState(ELiveStateKey.Sleep, out _sleepState);
            
            return UniTask.CompletedTask;
        }

        public bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out ELiveStateKey key, out float statePercent)
                   && key is ELiveStateKey.Trust or ELiveStateKey.Hunger
                   && statePercent < 0.4f;
        }

        public bool IsCanSleep(float bonusMinPercent = 0)
        {
            float minPercent = 0.3f + bonusMinPercent;
            
            Log.Info(this, "[IsCanSleep]" +
                                $" && ({_timeObserver.IsNightTime()}||{_sleepState?.GetPercent() < minPercent})" +
                                $" && {_sleepState?.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState?.Max}",
                Log.Type.CharacterCondition);

            return _sleepState != null && (_timeObserver.IsNightTime() || _sleepState.GetPercent() < minPercent) &&
                   _sleepState.Current + _sleepHealValue * _stoppingTicksToMaximumSleepValues < _sleepState.Max;
        }

        public bool IsCanExitWhenSleep()
        {
            _statesAnalytic.TryGetLowerSate(out ELiveStateKey lowerKey, out float lowerStatePercent);

            bool randomResult = Random.Range(0, 100) >= 50;

            Log.Info("[IsCanExitWhenSleep]" +
                          $" {lowerKey is ELiveStateKey.Trust}" +
                          $" && ({lowerStatePercent <= 0.4f})" +
                          $" && {randomResult}",
                Log.Type.CharacterCondition);

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

            Log.Info(this, "[CanShowNimbus]" +
                                $" {!_animationAnalytic.IsTransition}" +
                                $" && {isCorrectInteractionResult}" +
                                $" && {isCorrectMode}" +
                                $" && {isCorrectState}",
                Log.Type.CharacterCondition);

            return !_animationAnalytic.IsTransition && isCorrectInteractionResult && isCorrectMode && isCorrectState;
        }

        #endregion
    }
    
}