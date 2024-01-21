﻿using System;
using System.Collections;
using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Sleep : BehaviourNode
    {
        private readonly CharacterLiveStatesAnalytics _liveStateAnalytics;
        private readonly CharacterLiveStateStorage _liveStateStorage;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly TimeObserver _timeObserver;

        private readonly Character _character;
        private CharacterLiveState _sleepState;


        public BehaviorNode_Sleep()
        {
            _liveStateAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();
            _liveStateStorage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _character = Container.Instance.GetCharacter();
        }

        protected override void Run()
        {
            if (!IsCanSleep())
            {
                Debugging.Instance.Log($"Нода сна: отказ " +
                                       $"{!_timeObserver.IsNightTime()} " +
                                       $"{_liveStateAnalytics.CurrentLowerLiveStateKey != LiveStateKey.Sleep}",
                    Debugging.Type.BehaviorTree);

                Return(false);
            }
            else
            {
                Debugging.Instance.Log($"Нода сна: выбрано ", Debugging.Type.BehaviorTree);

                _liveStateStorage.TryGetCharacterLiveState(LiveStateKey.Sleep, out CharacterLiveState sleepState);
                _liveStateAnalytics.TryGetLowerSate(out LiveStateKey key, out float statePercent);

                if (sleepState.Current > 0.9f)
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> стаейт сна полный, возврат ", Debugging.Type.BehaviorTree);
                    Return(false);
                    return;
                }

                
                if (key is LiveStateKey.Trust && statePercent <= 0.4f && UnityEngine.Random.Range(0, 100) >= 50)
                {
                    Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТАРТ", Debugging.Type.BehaviorTree);
                    _coroutineRunner.StartRoutine(PlayExitAnimationRoutine());
                }

                _timeObserver.StartDayEvent += StopSleep;
                sleepState.ChangedEvent += SleepStateOnChangedEvent;
                _character.Animator.EnterToMode(CharacterAnimationMode.Sleep);
            }
        }

        private void SleepStateOnChangedEvent(float obj)
        {
            if (obj > 0.95f)
            {
                StopSleep();
            }
        }

        protected override void OnBreak()
        {
            Debugging.Instance.Log($"Нода сна: брейк ");
            base.OnBreak();
        }

        private IEnumerator PlayExitAnimationRoutine()
        {
            yield return new WaitUntil(() => _liveStateAnalytics.GetStatePercent(LiveStateKey.Sleep) >= 0.7f);
                    Debugging.Instance.Log($"Нода сна: выбрано -> прячется СТОП", Debugging.Type.BehaviorTree);
            _character.Animator.EnterToMode(CharacterAnimationMode.Sleep);
        }

        private void StopSleep()
        {
            Return(true);
        }

        private bool IsCanSleep()
        {
            return _timeObserver.IsNightTime() || _liveStateAnalytics.CurrentLowerLiveStateKey == LiveStateKey.Sleep;
        }
    }
}