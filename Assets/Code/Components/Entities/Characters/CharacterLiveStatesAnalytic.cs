using System;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;

namespace Code.Components.Entities.Characters
{
    public class CharacterLiveStatesAnalytic : CharacterComponent, IGameInitListener, IGameStartListener,
        IGameExitListener
    {
        private TimeObserver _timeObserver;
        private LiveStateStorage _storage;
        public LiveStateKey CurrentLowerLiveStateKey { get; private set; }

        public event Action<LiveStateKey> SwitchLowerStateKeyEvent;

        public void GameInit()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            SubscribeToEvents(true);
        }

        public void GameStart()
        {
            CheckLowerState();
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
            if (!keyValuePairs.Any())
            {
                Debugging.Instance.Log($"[CheckLowerState] return when try check lower state",
                    Debugging.Type.LiveState);
                return;
            }

            var lowerCharacterLiveState = keyValuePairs.First().Key;

            Debugging.Instance.Log(
                $"[CheckLowerState] try switch lower state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} " +
                $"{_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                Debugging.Type.LiveState);

            var resultState = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                ? LiveStateKey.None
                : lowerCharacterLiveState;

            if (resultState != CurrentLowerLiveStateKey)
            {
                Debugging.Instance.Log($"[CheckLowerState] {CurrentLowerLiveStateKey} switch {resultState}",
                    Debugging.Type.LiveState);
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
                Debugging.Instance.Log($"[TryGetLowerSate](true) -> {liveStateKey} {statePercent}",
                    Debugging.Type.LiveState);
                return true;
            }

            statePercent = 1;
            Debugging.Instance.Log($"[TryGetLowerSate](false) -> {liveStateKey} {statePercent}",
                Debugging.Type.LiveState);
            return false;
        }
    }
}