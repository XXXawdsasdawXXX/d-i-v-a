using System;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;

namespace Code.Components.Character.LiveState
{
    public class LiveStatesAnalytics :  IGameStartListener, IGameExitListener
    {
        private TimeObserver _timeObserver;
        private LiveStateStorage _storage;
        public LiveStateKey CurrentLowerLiveStateKey { get; private set; }
        public event Action<LiveStateKey> SwitchLowerStateKeyEvent;

        public void GameStart()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            CheckLowerState();

            SubscribeToEvents(true);
        }

        public void GameExit()
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


        private void CheckLowerState()
        {
            var keyValuePairs = _storage.LiveStates.OrderBy(kv => kv.Value.GetPercent());
            if(!keyValuePairs.Any())return;
            var lowerCharacterLiveState = keyValuePairs.First().Key;
            if (lowerCharacterLiveState != CurrentLowerLiveStateKey)
            {
                Debugging.Instance.Log(
                    $"Switch lover state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} {_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                    Debugging.Type.LiveState);
                CurrentLowerLiveStateKey = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                    ? LiveStateKey.None
                    : lowerCharacterLiveState;

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