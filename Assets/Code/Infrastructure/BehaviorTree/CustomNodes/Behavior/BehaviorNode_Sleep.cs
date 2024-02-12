using System.Collections;
using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Characters.Reactions;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Sleep : BaseNode
    {
        [Header("Character")] 
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CharacterLiveState _sleepState;

        [Header("Services")] 
        private readonly LiveStateStorage _liveStateStorage;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeObserver _timeObserver;


        public BehaviorNode_Sleep()
        {
            var character = Container.Instance.FindEntity<Character>();
            //character-------------------------------------------------------------------------------------------------
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            Container.Instance.FindStorage<LiveStateStorage>().TryGetLiveState(LiveStateKey.Sleep, out _sleepState);
            //services--------------------------------------------------------------------------------------------------
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            //nodes-----------------------------------------------------------------------------------------------------
        }

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

        private IEnumerator PlayExitAnimationRoutine()
        {
            _characterAnimator.EnterToMode(CharacterAnimationMode.None);
            yield return new WaitUntil(() => _statesAnalytic.GetStatePercent(LiveStateKey.Sleep) >= 0.7f);
            Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТОП", Debugging.Type.BehaviorTree);
            _characterAnimator.EnterToMode(CharacterAnimationMode.Sleep);
        }

        private void StopSleep()
        {
            Debugging.Instance.Log($"Нода сна: стоп сон", Debugging.Type.BehaviorTree);
            SubscribeToEvents(false);
            Return(true);
        }

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.StartDayEvent += StopSleep;
                _sleepState.ChangedEvent += OnChangedSleepStateValue;
            }
            else
            {
                _timeObserver.StartDayEvent -= StopSleep;
                _sleepState.ChangedEvent -= OnChangedSleepStateValue;
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
            return _timeObserver.IsNightTime() || _sleepState.GetPercent() < 0.5f;
        }
        
        private  bool IsCanExit()
        {
            _statesAnalytic.TryGetLowerSate(out LiveStateKey lowerKey, out var lowerStatePercent);
            return lowerKey is LiveStateKey.Trust && lowerStatePercent <= 0.4f && Random.Range(0, 100) >= 50;
        }

        #endregion
        
    }
}