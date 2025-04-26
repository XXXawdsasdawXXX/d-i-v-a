using System.Collections.Generic;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Services
{
    [Preserve]
    public class LiveStateTimer : IService, IInitListener, ISubscriber
    {
        private LiveStateStorage _storage;
        private TimeObserver _timeObserver;
        
        private ELiveStateKey _currentLowerLiveStateKey;

        public UniTask GameInitialize()
        {
            _timeObserver = Container.Instance.GetService<TimeObserver>();
            _storage = Container.Instance.FindStorage<LiveStateStorage>();

            return UniTask.CompletedTask;
        }

        public UniTask Subscribe()
        {
            _timeObserver.OnTicked += _onTimeObserverTick;

            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _timeObserver.OnTicked -= _onTimeObserverTick;
        }

        private void _onTimeObserverTick()
        {
#if DEBUGGING
            if (_storage.LiveStates == null)
            {
                Debugging.LogError(this,
                    $"[_onTimeObserverTick] storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }
#endif

            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in _storage.LiveStates)
            {
                if (_isCanUpdateLiveState(liveState))
                {
#if DEBUGGING
                    Debugging.Log(this,
                        $"[_onTimeObserverTick] update {liveState.Key} is healing {liveState.Value.IsHealing}",
                        Debugging.Type.LiveState);
#endif

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