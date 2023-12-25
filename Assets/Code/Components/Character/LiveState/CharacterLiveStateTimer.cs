using System;
using System.Collections.Generic;
using System.Linq;
using Code.Components.Character.Controllers;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterLiveStateTimer : MonoBehaviour, IGameStartListener, IGameExitListener
    {
        private CharacterLiveStateStorage _storage;
        private TimeObserver _timeObserver;
        
        private LiveStateKey  _currentLowerLiveStateKey;
        
       
        public void GameStart()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _storage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
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

 

        #region Editor

        public void LogStates()
        {
            if (_storage.LiveStates == null)
            {
                Debugging.Instance.ErrorLog($"_storage.LiveStates is null -> {_storage.LiveStates == null}");
                return;
            }
            foreach (var liveState in _storage.LiveStates)
            {
                Debugging.Instance.Log($"{liveState.Key} = {liveState.Value.Current} || {liveState.Value.GetPercent()}", Debugging.Type.LiveState);
            }
        }

        #endregion
        
        
   
    }
}