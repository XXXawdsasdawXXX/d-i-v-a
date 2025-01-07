﻿using System.Collections;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
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
        private readonly CharacterCondition _characterCondition;

        [Header("Node")] 
        private readonly SubNode_ReactionToVoice _subNode_reactionToVoice;

        [Header("Static values")] 
        private readonly LiveStateStorage _liveStateStorage;
        private readonly LiveStateRangePercentageValue _effectAwakeningValue;


        public BehaviourNode_Sleep()
        {
            Container.Instance.FindService<BehaviourTreeLoader>().AddProgressWriter(this);
       
            //character-------------------------------------------------------------------------------------------------
            Components.Entities.Diva diva = Container.Instance.FindEntity<Components.Entities.Diva>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _statesAnalytic = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _characterButton = diva.FindCommonComponent<ColliderButton>();
            
            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _tickCounter = new TickCounter(Container.Instance.FindConfig<TimeConfig>().Cooldown.Sleep);
            _characterCondition = Container.Instance.FindService<CharacterCondition>();
            
            //node------------------------------------------------------------------------------------------------------
            _subNode_reactionToVoice = new SubNode_ReactionToVoice();
            
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
                Debugging.Instance.Log(this, $"[run]", Debugging.Type.BehaviorTree);

                SubscribeToEvents(true);
                
                _sleepState?.SetHealUpdate();
                
                _divaAnimator.EnterToMode(EDivaAnimationMode.Sleep);

                if (_characterCondition.IsCanExitWhenSleep())
                {
                    Debugging.Instance.Log($"[run] -> exit anim routine", Debugging.Type.BehaviorTree);
                    
                    _coroutineRunner.StartRoutine(_playExitAnimationRoutine());
                }
            }
            else
            {
                Debugging.Instance.Log(this, $"[run] return", Debugging.Type.BehaviorTree);
                
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
            Debugging.Instance.Log(this, $"[break]");
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
            
            Debugging.Instance.Log(this, $"[run] end exit anim routine", Debugging.Type.BehaviorTree);
            
            _divaAnimator.EnterToMode(EDivaAnimationMode.Sleep);
        }

        private void _rouse()
        {
            Debugging.Instance.Log(this, $"[rouse]", Debugging.Type.BehaviorTree);
            
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
            
            Debugging.Instance.Log(this, $"stop sleep", Debugging.Type.BehaviorTree);
        }

        #endregion
        
        #region Save

        public void UpdateData(BehaviourTreeLoader.Data data)
        {
            data.SleepRemainingTick = _tickCounter.GetRemainingTick();
            
            Debugging.Instance.Log(this, $"[save]", Debugging.Type.BehaviorTree);
        }

        public void LoadData(BehaviourTreeLoader.Data data)
        {
            Debugging.Instance.Log(this, $"[load] -> SleepRemainingTick = {data.SleepRemainingTick}", Debugging.Type.BehaviorTree);
            
            if (data.SleepRemainingTick > 0)
            {
                _tickCounter.StartWait(data.SleepRemainingTick);
            }
        }

        #endregion
    }
}