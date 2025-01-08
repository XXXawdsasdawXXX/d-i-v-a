using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;

namespace Code.Infrastructure.Services
{
    public class LiveStateTimer : IService, IGameInitListener, IGameExitListener
    {
        private LiveStateStorage _storage;
        private TimeObserver _timeObserver;
        private ELiveStateKey _currentLowerLiveStateKey;

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
                Debugging.LogError(
                    $"[OnTimeObserverTick] storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }

            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in _storage.LiveStates)
            {
                if (IsCanUpdateLiveState(liveState))
                {
                    Debugging.Log(
                        $"[OnTimeObserverTick] update {liveState.Key} is healing {liveState.Value.IsHealing}",
                        Debugging.Type.LiveState);
                    liveState.Value.TimeUpdate();
                }
            }
        }

        private bool IsCantUpdateLiveState(KeyValuePair<ELiveStateKey, CharacterLiveState> liveState)
        {
            return (liveState.Key == ELiveStateKey.Trust && !liveState.Value.IsHealing &&
                    !_storage.IsEmptyState(ELiveStateKey.Hunger));
        }

        private bool IsCanUpdateLiveState(KeyValuePair<ELiveStateKey, CharacterLiveState> liveState)
        {
            return liveState.Key != ELiveStateKey.Trust;
        }
    }
}