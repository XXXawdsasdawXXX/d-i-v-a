using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;

namespace Code.Infrastructure.Services
{
    public class LiveStateTimer : IService, IGameInitListener, IGameExitListener
    {
        private LiveStateStorage _storage;
        private TimeObserver _timeObserver;
        private LiveStateKey _currentLowerLiveStateKey;

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
                Debugging.Instance.ErrorLog(
                    $"[OnTimeObserverTick] storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }

            foreach (var liveState in _storage.LiveStates)
            {
                if (IsCanUpdateLiveState(liveState))
                {
                    Debugging.Instance.Log(
                        $"[OnTimeObserverTick] update {liveState.Key} is healing {liveState.Value.IsHealing}",
                        Debugging.Type.LiveState);
                    liveState.Value.TimeUpdate();
                }
            }
        }

        private bool IsCantUpdateLiveState(KeyValuePair<LiveStateKey, CharacterLiveState> liveState)
        {
            return (liveState.Key == LiveStateKey.Trust && !liveState.Value.IsHealing &&
                    !_storage.IsEmptyState(LiveStateKey.Hunger));
        }

        private bool IsCanUpdateLiveState(KeyValuePair<LiveStateKey, CharacterLiveState> liveState)
        {
            return liveState.Key != LiveStateKey.Trust;
        }
    }
}