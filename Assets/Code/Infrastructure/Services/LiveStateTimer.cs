using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;

namespace Code.Infrastructure.Services
{
    public class LiveStateTimer : IService, IInitListener, IExitListener
    {
        private LiveStateStorage _storage;
        private TimeObserver _timeObserver;
        private ELiveStateKey _currentLowerLiveStateKey;

        public void GameInitialize()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();
         
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
                _timeObserver.OnTicked += _onTimeObserverTick;
            }
            else
            {
                _timeObserver.OnTicked -= _onTimeObserverTick;
            }
        }

        private void _onTimeObserverTick()
        {
            if (_storage.LiveStates == null)
            {
                Debugging.LogError(this, 
                    $"[_onTimeObserverTick] storage.LiveStates is null -> {_storage.LiveStates == null}");
              
                return;
            }

            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in _storage.LiveStates)
            {
                if (_isCanUpdateLiveState(liveState))
                {
                    Debugging.Log(this,
                        $"[_onTimeObserverTick] update {liveState.Key} is healing {liveState.Value.IsHealing}",
                        Debugging.Type.LiveState);
                  
                    liveState.Value.TimeUpdate();
                }
            }
        }

        private bool _isCanUpdateLiveState(KeyValuePair<ELiveStateKey, CharacterLiveState> liveState)
        {
            return liveState.Key != ELiveStateKey.Trust;
        }
    }
}