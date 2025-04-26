using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Entities.Diva
{
    public class DivaLiveStatesAnalytic : DivaComponent, IInitListener, IStartListener, ISubscriber
    {
        public event Action<ELiveStateKey> SwitchLowerStateKeyEvent;
      
        public ELiveStateKey CurrentLowerLiveStateKey { get; private set; }

        private TimeObserver _timeObserver;
        private LiveStateStorage _storage;

        public UniTask GameInitialize()
        {
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _timeObserver.OnTicked += _checkLowerState;
        }

        public UniTask GameStart()
        {
            _checkLowerState();
            
            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _timeObserver.OnTicked -= _checkLowerState;
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

#if DEBUGGING
                Log.Info(this, $"[TryGetLowerSate](true) -> {liveStateKey} {statePercent}", 
                    Log.Type.LiveState);
#endif
                
                return true;
            }

            statePercent = 1;

#if DEBUGGING
            Log.Info(this, $"[TryGetLowerSate](false) -> {liveStateKey} {statePercent}",
                Log.Type.LiveState);
#endif
            
            return false;
        }

        private void _checkLowerState()
        {
            IOrderedEnumerable<KeyValuePair<ELiveStateKey, CharacterLiveState>> keyValuePairs = 
                _storage.LiveStates.OrderBy(kv => kv.Value.GetPercent());
            
            ELiveStateKey lowerCharacterLiveState = keyValuePairs.First().Key;

#if DEBUGGING
            Log.Info(this, 
                $"[_checkLowerState] try switch lower state from {CurrentLowerLiveStateKey} to {lowerCharacterLiveState} " +
                $"{_storage.LiveStates[lowerCharacterLiveState].GetPercent() <= 0.4f}",
                Log.Type.LiveState);
#endif

            ELiveStateKey resultState = _storage.LiveStates[lowerCharacterLiveState].GetPercent() > 0.4f
                ? ELiveStateKey.None
                : lowerCharacterLiveState;

            if (resultState != CurrentLowerLiveStateKey)
            {
#if DEBUGGING
                Log.Info(this, $"[_checkLowerState] {CurrentLowerLiveStateKey} switch {resultState}",
                    Log.Type.LiveState);
#endif
               
                CurrentLowerLiveStateKey = resultState;
                
                SwitchLowerStateKeyEvent?.Invoke(CurrentLowerLiveStateKey);
            }
        }
    }
}