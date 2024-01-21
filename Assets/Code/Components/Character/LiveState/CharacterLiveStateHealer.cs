using Code.Components.Character.AnimationReader.State;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character.LiveState
{
    public class CharacterLiveStateHealer : MonoBehaviour, IGameStartListener, IGameExitListener
        {
        private CharacterAnimationStateObserver _animationState;
        private CharacterAnimator _animationMode;
        
        private CharacterLiveStateStorage _storage;
        private TimeObserver _timeObserver;
        
        private LiveStateKey  _currentLowerLiveStateKey;


        public void GameStart()
        {
            _animationState = Container.Instance.GetCharacter().AnimationStateObserver;
            _animationMode = Container.Instance.GetCharacter().Animator;
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
            if (_animationMode.Mode == CharacterAnimationMode.Sleep)
            {
                
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
