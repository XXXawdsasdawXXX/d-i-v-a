using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
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
        public ELiveStateKey CurrentLowerLiveStateKey { get; private set; }

        public event Action<ELiveStateKey> SwitchLowerStateKeyEvent;

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
            IOrderedEnumerable<KeyValuePair<ELiveStateKey, CharacterLiveState>> keyValuePairs = _storage.LiveStates.OrderBy(kv => kv.Value.GetPercent());
         
            if (!keyValuePairs.Any())
            {
                Debugging.Instance.Log(this, $"[CheckLowerState] return when try check lower state",
                    Debugging.Type.LiveState);
                return;
            }

            ELiveStateKey lowerCharacterLiveState = keyValuePairs.First().Key;

            Debugging.Instance.Log(this, 
                $"[CheckLowerState] try switch lower state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} " +
                $"{_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                Debugging.Type.LiveState);

            ELiveStateKey resultState = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                ? ELiveStateKey.None
                : lowerCharacterLiveState;

            if (resultState != CurrentLowerLiveStateKey)
            {
                Debugging.Instance.Log(this, $"[CheckLowerState] {CurrentLowerLiveStateKey} switch {resultState}",
                    Debugging.Type.LiveState);
                CurrentLowerLiveStateKey = resultState;
                SwitchLowerStateKeyEvent?.Invoke(CurrentLowerLiveStateKey);
            }
        }

        public float GetStatePercent(ELiveStateKey liveStateKey)
        {
            if (_storage != null && _storage.TryGetLiveState(liveStateKey, out CharacterLiveState characterLiveState))
            {
                return characterLiveState.Current;
            }

            return 0;
        }

        public bool TryGetLowerSate(out ELiveStateKey liveStateKey, out float statePercent)
        {
            liveStateKey = CurrentLowerLiveStateKey;
            if (_storage != null && _storage.TryGetLiveState(liveStateKey, out CharacterLiveState characterLiveState))
            {
                statePercent = characterLiveState.GetPercent();
                Debugging.Instance.Log(this, $"[TryGetLowerSate](true) -> {liveStateKey} {statePercent}",
                    Debugging.Type.LiveState);
                return true;
            }

            statePercent = 1;
            Debugging.Instance.Log(this, $"[TryGetLowerSate](false) -> {liveStateKey} {statePercent}",
                Debugging.Type.LiveState);
            return false;
        }
    }
}