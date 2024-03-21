using System;
using Code.Infrastructure.GameLoop;
using UnityEngine;


namespace Code.Services
{
    public class InteractionObserver_ReturnAfterAbsence : InteractionObserver,IGameTickListener
    {
        private const float NEEDED_MIN = 10;
        private float _absenceTime;
        private bool _isAbsence;

        public event Action<float> UserReturnEvent;


        public void GameTick()
        {
            if (Input.anyKeyDown)
            {
                if (_isAbsence)
                {
                    _isAbsence = false;
                    UserReturnEvent?.Invoke(_absenceTime);
                }

                _absenceTime = 0;
            }

            _absenceTime += Time.deltaTime;
            if (_absenceTime >= NEEDED_MIN * 60 && !_isAbsence)
            {
                _isAbsence = true;
                InvokeInteractionEvent();
            }
        }
    }
}