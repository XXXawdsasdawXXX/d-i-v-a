using System.Collections;
using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Sleep : BaseNode
    {
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;

        private readonly LiveStateStorage _liveStateStorage;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeObserver _timeObserver;

        private readonly CharacterLiveState _sleepState;


        public BehaviorNode_Sleep()
        {
            var character = Container.Instance.FindEntity<Character>();
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            Container.Instance.FindStorage<LiveStateStorage>().TryGetLiveState(LiveStateKey.Sleep, out _sleepState);
        }

        protected override void Run()
        {
            if(IsCanSleep())
            {
                Debugging.Instance.Log($"Нода сна: выбрано ", Debugging.Type.BehaviorTree);

                _statesAnalytic.TryGetLowerSate(out LiveStateKey key, out float statePercent);

                if (_sleepState.GetPercent() > 0.9f)
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> стаейт сна полный, возврат ",
                        Debugging.Type.BehaviorTree);
                    Return(false);
                    return;
                }

                _sleepState?.SetHealUpdate();

                if (key is LiveStateKey.Trust && statePercent <= 0.4f && UnityEngine.Random.Range(0, 100) >= 50)
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТАРТ", Debugging.Type.BehaviorTree);
                    _coroutineRunner.StartRoutine(PlayExitAnimationRoutine());
                }

                SubscribeToEvents(true);
                _characterAnimator.EnterToMode(CharacterAnimationMode.Sleep);
            }
            else
            {
                Debugging.Instance.Log($"Нода сна: отказ " +
                                       $"{!_timeObserver.IsNightTime()} " +
                                       $"{_statesAnalytic.CurrentLowerLiveStateKey != LiveStateKey.Sleep}",
                    Debugging.Type.BehaviorTree);

                Return(false);
            }
        }

        private void SleepStateOnChangedEvent(float obj)
        {
            if (_sleepState.GetPercent() > 0.9f)
            {
                Debugging.Instance.Log($"Нода сна: сон на максимальном значение ");
                StopSleep();
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

        private bool IsCanSleep()
        {
            return _timeObserver.IsNightTime() || _sleepState.GetPercent() < 0.5f;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.StartDayEvent += StopSleep;
                _sleepState.ChangedEvent += SleepStateOnChangedEvent;
            }
            else
            {
                _timeObserver.StartDayEvent -= StopSleep;
                _sleepState.ChangedEvent -= SleepStateOnChangedEvent;
            }
        }
    }
}