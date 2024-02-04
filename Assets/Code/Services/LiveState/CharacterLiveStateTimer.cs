using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character.LiveState
{
    public class CharacterLiveStateTimer : ILiveStateLogic, IGameInitListener, IGameExitListener
    {
        private LiveStateStorage _storage;
        private TimeObserver _timeObserver;
        
        private LiveStateKey  _currentLowerLiveStateKey;
        
       
        public void GameInit()
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
                _timeObserver.TickEvent += OnTimeObserverTick;
            }
            else
            {
                _timeObserver.TickEvent -= OnTimeObserverTick;
            }
        }

        private void OnTimeObserverTick()
        {
            if (_storage.LiveStates == null)
            {
                Debugging.Instance.ErrorLog($"_storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }
            foreach (var liveState in _storage.LiveStates)
            {
                liveState.Value.TimeUpdate();
            }
        }

    }
}