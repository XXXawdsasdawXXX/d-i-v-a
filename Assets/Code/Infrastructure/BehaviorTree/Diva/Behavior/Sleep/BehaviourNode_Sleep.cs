using System.Collections;
using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public partial class BehaviourNode_Sleep : BaseNode_Root, IProgressWriterNode
    {
        [Header("Character")] 
        private readonly DivaAnimator _divaAnimator;
        private readonly DivaLiveStatesAnalytic _statesAnalytic;
        private readonly CharacterLiveState _sleepState;
        private readonly ColliderButton _characterButton;

        [Header("Services")] 
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeObserver _timeObserver;
        private readonly TickCounter _tickCounter;
        private readonly DivaCondition _divaCondition;
        
        [Header("Static values")] 
        private readonly LiveStateStorage _liveStateStorage;
        private readonly LiveStateRangePercentageValue _effectAwakeningValue;


        public BehaviourNode_Sleep()
        {
            Container.Instance.FindService<BehaviourTreeLoader>().AddProgressWriter(this);

            //character-------------------------------------------------------------------------------------------------
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _statesAnalytic = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _characterButton = diva.FindCommonComponent<ColliderButton>();

            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _tickCounter = new TickCounter(Container.Instance.FindConfig<TimeConfig>().Cooldown.Sleep);
            _divaCondition = Container.Instance.FindService<DivaCondition>();

            //static values---------------------------------------------------------------------------------------------
            LiveStateConfig liveStateConfig = Container.Instance.FindConfig<LiveStateConfig>();
            _effectAwakeningValue = liveStateConfig.Awakening;
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _liveStateStorage.TryGetLiveState(ELiveStateKey.Sleep, out _sleepState);
        }

        #region Live cycle

        protected override void Run()
        {
            if (IsCanRun())
            {
#if DEBUGGING
                Debugging.Log(this, $"[run]", Debugging.Type.BehaviorTree);
#endif
                SubscribeToEvents(true);

                _sleepState?.SetHealUpdate();

                _divaAnimator.EnterToMode(EDivaAnimationMode.Sleep);

                if (_divaCondition.IsCanExitWhenSleep())
                {
#if DEBUGGING
                    Debugging.Log($"[run] Exit anim routine.", Debugging.Type.BehaviorTree);
#endif
                    _coroutineRunner.StartRoutine(_playExitAnimationRoutine());
                }
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, $"[run] Return.", Debugging.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _divaCondition.IsCanSleep();
        }

        protected override void OnBreak()
        {
            _sleepState?.SetDefaultUpdate();
#if DEBUGGING
            Debugging.Log(this, $"[break]");
#endif
            base.OnBreak();
        }

        public override void InvokeCallback(BaseNode node, bool success)
        {
            if (node is SubNode_ReactionToVoice)
            {
                _rouse();
            }

            base.InvokeCallback(node, success);
        }

        #endregion

        #region Unique methods

        private IEnumerator _playExitAnimationRoutine()
        {
            _divaAnimator.EnterToMode(EDivaAnimationMode.None);

            yield return new WaitUntil(() => _statesAnalytic.GetStatePercent(ELiveStateKey.Sleep) >= 0.7f);
#if DEBUGGING
            Debugging.Log(this, $"[_playExitAnimationRoutine] End.", Debugging.Type.BehaviorTree);
#endif
            _divaAnimator.EnterToMode(EDivaAnimationMode.Sleep);
        }

        private void _rouse()
        {
#if DEBUGGING
            Debugging.Log(this, $"[_rouse]", Debugging.Type.BehaviorTree);
#endif
            _liveStateStorage.AddPercentageValue(_effectAwakeningValue);

            _stopSleep(delay: 1.5f);
        }

        private void _stopSleep(float delay = 0)
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

#if DEBUGGING
            Debugging.Log(this, "[_stop sleep]", Debugging.Type.BehaviorTree);
#endif
        }

        #endregion

        #region Save

        public void UpdateData(BehaviourTreeLoader.Data data)
        {
            data.SleepRemainingTick = _tickCounter.GetRemainingTick();

#if DEBUGGING
            Debugging.Log(this, "[Save]", Debugging.Type.BehaviorTree);
#endif
        }

        public void LoadData(BehaviourTreeLoader.Data data)
        {
#if DEBUGGING
            Debugging.Log(this, $"[Load] -> SleepRemainingTick = {data.SleepRemainingTick}.", Debugging.Type.BehaviorTree);
#endif

            if (data.SleepRemainingTick > 0)
            {
                _tickCounter.StartWait(data.SleepRemainingTick);
            }
        }

        #endregion
    }
}