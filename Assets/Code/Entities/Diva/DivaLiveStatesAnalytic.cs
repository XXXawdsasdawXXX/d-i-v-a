using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;

namespace Code.Entities.Diva
{
    public class DivaLiveStatesAnalytic : DivaComponent, IInitListener, IStartListener,
        IExitListener
    {
        private TimeObserver _timeObserver;
        private LiveStateStorage _storage;
        public ELiveStateKey CurrentLowerLiveStateKey { get; private set; }

        public event Action<ELiveStateKey> SwitchLowerStateKeyEvent;

        public void GameInitialize()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            _subscribeToEvents(true);
        }

        public void GameStart()
        {
            _checkLowerState();
        }

        public void GameExit()
        {
            _subscribeToEvents(false);
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
               
                Debugging.Log(this, $"[TryGetLowerSate](true) -> {liveStateKey} {statePercent}",
                    Debugging.Type.LiveState);
                
                return true;
            }

            statePercent = 1;
       
            Debugging.Log(this, $"[TryGetLowerSate](false) -> {liveStateKey} {statePercent}",
                Debugging.Type.LiveState);
            
            return false;
        }
        
        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.OnTicked += _checkLowerState;
            }
            else
            {
                _timeObserver.OnTicked -= _checkLowerState;
            }
        }
        
        private void _checkLowerState()
        {
            IOrderedEnumerable<KeyValuePair<ELiveStateKey, CharacterLiveState>> keyValuePairs = _storage.LiveStates.OrderBy(kv => kv.Value.GetPercent());
         
            if (!keyValuePairs.Any())
            {
                Debugging.Log(this, $"[CheckLowerState] return when try check lower state",
                    Debugging.Type.LiveState);
                return;
            }

            ELiveStateKey lowerCharacterLiveState = keyValuePairs.First().Key;

            Debugging.Log(this, 
                $"[CheckLowerState] try switch lower state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} " +
                $"{_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                Debugging.Type.LiveState);

            ELiveStateKey resultState = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                ? ELiveStateKey.None
                : lowerCharacterLiveState;

            if (resultState != CurrentLowerLiveStateKey)
            {
                Debugging.Log(this, $"[CheckLowerState] {CurrentLowerLiveStateKey} switch {resultState}",
                    Debugging.Type.LiveState);
                CurrentLowerLiveStateKey = resultState;
                SwitchLowerStateKeyEvent?.Invoke(CurrentLowerLiveStateKey);
            }
        }

    }
}