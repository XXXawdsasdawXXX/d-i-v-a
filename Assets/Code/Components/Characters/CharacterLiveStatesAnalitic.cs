﻿using System;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Components.Character.LiveState
{
    public class LiveStatesAnalytics
    {
        private readonly TimeObserver _timeObserver;
        private readonly LiveStateStorage _storage;
        public LiveStateKey CurrentLowerLiveStateKey { get; private set; }
        public event Action<LiveStateKey> SwitchLowerStateKeyEvent;

        public LiveStatesAnalytics()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            Debugging.Instance.Log($"LiveStatesAnalytics construct", Debugging.Type.LiveState);

            SubscribeToEvents(true);
        }

        ~LiveStatesAnalytics()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.TickEvent += CheckLowerState;
            }
            else
            {
                _timeObserver.TickEvent -= CheckLowerState;
            }
        }


        public void CheckLowerState()
        {
            var keyValuePairs = _storage.LiveStates.OrderBy(kv => kv.Value.GetPercent());
            if (!keyValuePairs.Any())
            {
                Debugging.Instance.Log($"return when try check lower state", Debugging.Type.LiveState);
                return;
            }

            var lowerCharacterLiveState = keyValuePairs.First().Key;
            
            Debugging.Instance.Log(
                $"try switch lower state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} " +
                $"{_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                Debugging.Type.LiveState);

            var resultState = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                ? LiveStateKey.None
                : lowerCharacterLiveState;

            if (resultState != CurrentLowerLiveStateKey)
            {
                Debugging.Instance.Log($"{CurrentLowerLiveStateKey} switch {resultState}", Debugging.Type.LiveState);
                CurrentLowerLiveStateKey = resultState;
                SwitchLowerStateKeyEvent?.Invoke(CurrentLowerLiveStateKey);
            }
        }

        public float GetStatePercent(LiveStateKey liveStateKey)
        {
            if (_storage != null && _storage.TryGetLiveState(liveStateKey, out var characterLiveState))
            {
                return characterLiveState.Current;
            }

            return 0;
        }

        public bool TryGetLowerSate(out LiveStateKey liveStateKey, out float statePercent)
        {
            liveStateKey = CurrentLowerLiveStateKey;
            if (_storage != null && _storage.TryGetLiveState(liveStateKey, out var characterLiveState))
            {
                statePercent = characterLiveState.GetPercent();
                Debugging.Instance.Log(
                    $"try get lower state -> {liveStateKey} {statePercent} (true)",
                    Debugging.Type.LiveState);
                return true;
            }

            statePercent = 1;
            Debugging.Instance.Log(
                $"try get lower state -> {liveStateKey} {statePercent} (false)",
                Debugging.Type.LiveState);
            return false;
        }
    }
}