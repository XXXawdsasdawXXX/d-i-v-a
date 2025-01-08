using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_ReturnAfterAbsence : InteractionObserver, 
        IGameInitListener,
        IGameUpdateListener
    {
        private float _userStandStillSecond;
        private float _absenceSecond;

        public bool IsAbsence { get; private set; }

        public event Action<float> UserReturnEvent;

        public void GameInit()
        {
            _userStandStillSecond = Container.Instance.FindConfig<TimeConfig>().Duration.UserStandStillSecond;
        }

        public void GameUpdate()
        {
            if (Input.anyKeyDown)
            {
                if (IsAbsence)
                {
                    IsAbsence = false;
                    UserReturnEvent?.Invoke(_absenceSecond);
                }

                _absenceSecond = 0;
            }

            _absenceSecond += Time.deltaTime;
            if (_absenceSecond >= _userStandStillSecond * 60 && !IsAbsence)
            {
                IsAbsence = true;
                InvokeInteractionEvent();
            }
        }
    }
}