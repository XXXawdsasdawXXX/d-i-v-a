using System.Collections;
using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Sleep : BaseNode_Root
    {
        [Header("Character")] 
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CharacterLiveState _sleepState;
        private readonly ColliderButton _characterButton;

        [Header("Services")] 
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeObserver _timeObserver;
        private readonly TickCounter _tickCounter;
        private readonly MicrophoneAnalyzer _microphoneAnalyzer;
        
        [Header("Node")] 
        private readonly SubNode_ReactionToVoice _subNode_reactionToVoice;
        
        [Header("Values")] 
        private readonly LiveStateStorage _liveStateStorage;
        private readonly LiveStateRangePercentageValue _effectAwakeningValue;


        public BehaviorNode_Sleep()
        {
            var character = Container.Instance.FindEntity<DIVA>();
            //character-------------------------------------------------------------------------------------------------
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _characterButton = character.FindCommonComponent<ColliderButton>();
            Container.Instance.FindStorage<LiveStateStorage>().TryGetLiveState(LiveStateKey.Sleep, out _sleepState);
            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _tickCounter = new TickCounter(Container.Instance.FindConfig<TimeConfig>().Cooldown.Sleep);
            _microphoneAnalyzer = Container.Instance.FindService<MicrophoneAnalyzer>();
            //node------------------------------------------------------------------------------------------------------
            _subNode_reactionToVoice = new SubNode_ReactionToVoice();
            //values----------------------------------------------------------------------------------------------------
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _effectAwakeningValue = Container.Instance.FindConfig<LiveStateConfig>().Awakening;
        }

        #region Base methods
        
        protected override void Run()
        {
            if (IsCanSleep())
            {
                Debugging.Instance.Log($"Нода сна: выбрано ", Debugging.Type.BehaviorTree);

                SubscribeToEvents(true);
                _sleepState?.SetHealUpdate();
                _characterAnimator.EnterToMode(CharacterAnimationMode.Sleep);

                if (IsCanExit())
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТАРТ", Debugging.Type.BehaviorTree);
                    _coroutineRunner.StartRoutine(PlayExitAnimationRoutine());
                }
            }
            else
            {
                Debugging.Instance.Log($"Нода сна: отказ ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override void OnBreak()
        {
            _sleepState?.SetDefaultUpdate();
            Debugging.Instance.Log($"Нода сна: брейк ");
            base.OnBreak();
        }

        public override void InvokeCallback(BaseNode node, bool success)
        {
            if (node is SubNode_ReactionToVoice)
            {
                WakeUp();
            }
            base.InvokeCallback(node, success);
        }

        #endregion

        #region Unique methods
        
        private IEnumerator PlayExitAnimationRoutine()
        {
            _characterAnimator.EnterToMode(CharacterAnimationMode.None);
            yield return new WaitUntil(() => _statesAnalytic.GetStatePercent(LiveStateKey.Sleep) >= 0.7f);
            Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТОП", Debugging.Type.BehaviorTree);
            _characterAnimator.EnterToMode(CharacterAnimationMode.Sleep);
        }

        private void WakeUp()
        {
            Debugging.Instance.Log($"Нода сна: разбудили", Debugging.Type.BehaviorTree);
            _liveStateStorage.AddPercentageValue(_effectAwakeningValue);
            StopSleep(delay: 1.5f);
        }

        private void StopSleep(float delay = 0)
        {
            if (delay == 0)
            {
                _tickCounter.StartWait();
                Return(true);
            }
            else
            {
                _coroutineRunner.StartActionWithDelay(() =>
                {
                    _tickCounter.StartWait();
                    Return(true);
                },delay);
            }
            SubscribeToEvents(false);
            Debugging.Instance.Log($"Нода сна: стоп сон", Debugging.Type.BehaviorTree);
        }
        
        #endregion
        
        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterButton.SeriesOfClicksEvent += OnClickSeries;
                _timeObserver.StartDayEvent += WakeUp;
                _sleepState.ChangedEvent += OnChangedSleepStateValue;
                _microphoneAnalyzer.MaxDecibelRecordedEvent += OnMaxDecibelRecorder;
            }
            else
            {
                _characterButton.SeriesOfClicksEvent -= OnClickSeries;
                _timeObserver.StartDayEvent -= WakeUp;
                _sleepState.ChangedEvent -= OnChangedSleepStateValue;
                _microphoneAnalyzer.MaxDecibelRecordedEvent -= OnMaxDecibelRecorder;
            }
        }

        private void OnMaxDecibelRecorder()
        {
           RunNode(_subNode_reactionToVoice);
        }

        private void OnClickSeries(int clickCount)
        {
            if (clickCount >= 5)
            {
                WakeUp();
            }
        }


        private void OnChangedSleepStateValue(float sleepValue)
        {
            if (_sleepState.GetPercent() > 0.9f)
            {
                Debugging.Instance.Log($"Нода сна: сон на максимальном значение ");
                StopSleep();
            }
        }

        #endregion

        #region Conditions

        private bool IsCanSleep()
        {
            return _tickCounter.IsWaited && (_timeObserver.IsNightTime() || _sleepState.GetPercent() < 0.5f);
        }

        private bool IsCanExit()
        {
            _statesAnalytic.TryGetLowerSate(out LiveStateKey lowerKey, out var lowerStatePercent);
            return lowerKey is LiveStateKey.Trust && lowerStatePercent <= 0.4f && Random.Range(0, 100) >= 50;
        }

        #endregion
    }
}