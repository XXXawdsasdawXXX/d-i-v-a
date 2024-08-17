using System.Collections;
using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.Character.Sub;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Character.Behavior
{
    public class BehaviourNode_Sleep : BaseNode_Root, IProgressWriterNode
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
        private readonly CharacterCondition _characterCondition;

        [Header("Node")] 
        private readonly SubNode_ReactionToVoice _subNode_reactionToVoice;

        [Header("Static values")] 
        private readonly LiveStateStorage _liveStateStorage;
        private readonly LiveStateRangePercentageValue _effectAwakeningValue;


        public BehaviourNode_Sleep()
        {
            Container.Instance.FindService<BehaviourTreeLoader>().AddProgressWriter(this);
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
            _characterCondition = Container.Instance.FindService<CharacterCondition>();
            //node------------------------------------------------------------------------------------------------------
            _subNode_reactionToVoice = new SubNode_ReactionToVoice();
            //static values---------------------------------------------------------------------------------------------
            var liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            _effectAwakeningValue = liveStateConfig.Awakening;
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        #region Live cycle

        protected override void Run()
        {
            if (IsCanRun())
            {
                Debugging.Instance.Log($"Нода сна: выбрано", Debugging.Type.BehaviorTree);

                SubscribeToEvents(true);
                _sleepState?.SetHealUpdate();
                _characterAnimator.EnterToMode(CharacterAnimationMode.Sleep);

                if (_characterCondition.IsCanExitWhenSleep())
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТАРТ", Debugging.Type.BehaviorTree);
                    _coroutineRunner.StartRoutine(PlayExitAnimationRoutine());
                }
            }
            else
            {
                Debugging.Instance.Log($"Нода сна: отказ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _characterCondition.IsCanSleep();
        }

        protected override void OnBreak()
        {
            _sleepState?.SetDefaultUpdate();
            Debugging.Instance.Log($"Нода сна: брейк");
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
                }, delay);
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
                Debugging.Instance.Log($"Нода сна: сон на максимальном значение", Debugging.Type.BehaviorTree);
                StopSleep();
            }
        }

        #endregion

        #region Conditions

        #endregion

        #region Save

        public void UpdateData(BehaviourTreeLoader.Data data)
        {
            data.SleepRemainingTick = _tickCounter.GetRemainingTick();
            Debugging.Instance.Log($"Нода сна: обновила дату на сохранение", Debugging.Type.BehaviorTree);
        }

        public void LoadData(BehaviourTreeLoader.Data data)
        {
            Debugging.Instance.Log($"Нода сна: загрузила тики {data.SleepRemainingTick}", Debugging.Type.BehaviorTree);
            if (data.SleepRemainingTick > 0)
            {
                _tickCounter.StartWait(data.SleepRemainingTick);
            }
        }

        #endregion
    }
}