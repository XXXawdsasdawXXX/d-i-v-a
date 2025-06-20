﻿using System.Collections.Generic;
using Code.Data;
using Code.Game.Services.LiveState;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.Services
{
    [Preserve]
    public class LiveStateTimer : IService, IInitializeListener, ISubscriber
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

        public void Subscribe()
        {
            _timeObserver.OnTicked += _onTimeObserverTick;
        }

        public void Unsubscribe()
        {
            _timeObserver.OnTicked -= _onTimeObserverTick;
        }

        private void _onTimeObserverTick()
        {
            if (_storage.LiveStates == null)
            {
                Log.Error(this,
                    $"[_onTimeObserverTick] storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }

            foreach (KeyValuePair<ELiveStateKey, CharacterLiveState> liveState in _storage.LiveStates)
            {
                if (_isCanUpdateLiveState(liveState))
                {
                    Log.Info(this,
                        $"[_onTimeObserverTick] update {liveState.Key} is healing {liveState.Value.IsHealing}",
                        Log.Type.LiveState);

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