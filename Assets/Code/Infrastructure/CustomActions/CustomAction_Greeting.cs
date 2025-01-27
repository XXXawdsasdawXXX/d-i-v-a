using System;
using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Infrastructure.Services;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    //todo #46 вернуть функционал
    public class CustomAction_Greeting : CustomAction, IProgressWriter, IInitListener, IExitListener, IToggle
    {
        [Header("Services")] 
        private AudioEventsService _audioEventsService;
        private TimeObserver _timeObserver;

        [Header("Character")] 
        private ColliderButton _colliderButton;

        [Header("Values")] private bool _isAlreadySaidHi;
        private bool _isFirstVisit;

        private bool _isActive;

        public UniTask GameInitialize()
        {
            _audioEventsService = Container.Instance.FindService<AudioEventsService>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _colliderButton = diva.FindCommonComponent<ColliderButton>();
            
            Active();
            
            return UniTask.CompletedTask;
        }

        public void GameExit()
        {
            Disable();
        }

        public UniTask LoadProgress(PlayerProgressData playerProgress)
        {
            _isAlreadySaidHi = playerProgress.CustomActions.IsAlreadySaidHi;
          
            if (_isAlreadySaidHi && _isFirstVisit)
            {
                _isAlreadySaidHi = false;
            }
#if DEBUGGING
            Debugging.Log(this, $"[Load] _isAlreadySaidHi = {_isAlreadySaidHi}", Debugging.Type.CustomAction);
#endif
            return UniTask.CompletedTask;
        }

        public void SaveProgress(PlayerProgressData playerProgress)
        {
            playerProgress.CustomActions.IsAlreadySaidHi = _isAlreadySaidHi;
#if DEBUGGING
            Debugging.Log(this, $"[Save] _isAlreadySaidHi = {_isAlreadySaidHi}", Debugging.Type.CustomAction);
#endif
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Greeting;
        }
        
        public void Active(Action OnTurnedOn = null)
        {
#if DEBUGGING
            Debugging.Log(this, $"[Active] is can = {!_isActive}", Debugging.Type.CustomAction);
#endif
            if (!_isActive)
            {
                _isActive = true;
                
                _subscribeToEvents(true);
            }
        }

        public void Disable(Action onTurnedOff = null)
        {
#if DEBUGGING
            Debugging.Log(this, $"[Disable] is can = {_isActive}", Debugging.Type.CustomAction);
#endif
            if (_isActive)
            {
                _isActive = false;
                
                _subscribeToEvents(false);
            }
        }


        private void _subscribeToEvents(bool flag)
        {
            /*if(flag)
            {
                _timeObserver.InitTimeEvent += OnInitTime;
                _colliderButton.DownEvent += TrySayHi;
            }
            else
            {
                _timeObserver.InitTimeEvent -= OnInitTime;
                _colliderButton.DownEvent -= TrySayHi;
            }*/
        }

        private void _trySayHi(Vector2 _)
        {
            if (_isAlreadySaidHi || !_isActive)
            {
#if DEBUGGING
                Debugging.Log(this, "[TrySayHi] (_isAlreadySaidHi || !_isActive)", Debugging.Type.CustomAction);
#endif
                return;
            }
            
            _isAlreadySaidHi = true;
            _audioEventsService.PlayAudio(EAudioEventType.Hi);
            
#if DEBUGGING
            Debugging.Log(this, "[TrySayHi] Say", Debugging.Type.CustomAction);
#endif
        }

        private void _onInitTime(bool isFirstVisit)
        {
#if DEBUGGING
            Debugging.Log(this, $"[OnInitTime] _isAlreadySaidHi = {_isAlreadySaidHi} isFirstVisit = {isFirstVisit}",
                Debugging.Type.CustomAction);
#endif
            if (_isAlreadySaidHi && isFirstVisit)
            {
                _isAlreadySaidHi = false;
            }
        }

    }
}