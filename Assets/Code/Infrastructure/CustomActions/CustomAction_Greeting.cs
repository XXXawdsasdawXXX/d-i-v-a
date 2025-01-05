using System;
using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.SavedData;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    //todo сейчас не активна, возможно стоит вырезать
    public class CustomAction_Greeting : CustomAction, IProgressWriter, IGameInitListener, IGameExitListener, IToggle
    {
        [Header("Services")] 
        private AudioEventsService _audioEventsService;
        private TimeObserver _timeObserver;

        [Header("Character")] 
        private ColliderButton _colliderButton;

        [Header("Values")] private bool _isAlreadySaidHi;
        private bool _isFirstVisit;

        private bool _isActive;

        public void GameInit()
        {
            _audioEventsService = Container.Instance.FindService<AudioEventsService>();
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            Diva diva = Container.Instance.FindEntity<Diva>();
            _colliderButton = diva.FindCommonComponent<ColliderButton>();
            Active();
        }

        public void GameExit()
        {
            Disable();
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            _isAlreadySaidHi = playerProgress.CustomActions.IsAlreadySaidHi;
            if (_isAlreadySaidHi && _isFirstVisit)
            {
                _isAlreadySaidHi = false;
            }

            Debugging.Instance.Log($"[Greeting][LoadProgress] _isAlreadySaidHi = {_isAlreadySaidHi}",
                Debugging.Type.CustomAction);
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            playerProgress.CustomActions.IsAlreadySaidHi = _isAlreadySaidHi;
            Debugging.Instance.Log($"[Greeting][UpdateProgress] _isAlreadySaidHi = {_isAlreadySaidHi}",
                Debugging.Type.CustomAction);
        }

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Greeting;
        }

        #region Events

        private void SubscribeToEvents(bool flag)
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

        private void TrySayHi(Vector2 _)
        {
            Debugging.Instance.Log($"[Greeting][TrySayHi] Trying", Debugging.Type.CustomAction);
            if (_isAlreadySaidHi || !_isActive)
            {
                Debugging.Instance.Log($"[Greeting][TrySayHi] is return", Debugging.Type.CustomAction);
                return;
            }

            _isAlreadySaidHi = true;
            _audioEventsService.PlayAudio(EAudioEventType.Hi);
            Debugging.Instance.Log($"[Greeting][TrySayHi] is say", Debugging.Type.CustomAction);
        }

        private void OnInitTime(bool isFirstVisit)
        {
            Debugging.Instance.Log(
                $"[Greeting][OnInitTime] _isAlreadySaidHi = {_isAlreadySaidHi} isFirstVisit = {isFirstVisit}",
                Debugging.Type.CustomAction);
            if (_isAlreadySaidHi && isFirstVisit)
            {
                _isAlreadySaidHi = false;
            }
        }

        public void Active(Action OnTurnedOn = null)
        {
            Debugging.Instance.Log($"[Greeting][On] is can = {!_isActive}", Debugging.Type.CustomAction);
            if (!_isActive)
            {
                _isActive = true;
                SubscribeToEvents(true);
            }
        }

        public void Disable(Action onTurnedOff = null)
        {
            Debugging.Instance.Log($"[Greeting][Off] is can = {_isActive}", Debugging.Type.CustomAction);
            if (_isActive)
            {
                _isActive = false;
                SubscribeToEvents(false);
            }
        }

        #endregion
    }
}