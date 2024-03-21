using System;
using Code.Infrastructure.GameLoop;
using UnityEngine;


namespace Code.Services
{
    public class Interaction_ReturnAfterAbsence : InteractionObserver,IGameTickListener
    {
        private const float NEEDED_MIN = 10;
        private float _absenceSecond;
        private bool _isAbsence;

        public event Action<float> UserReturnEvent;


        public void GameTick()
        {
            if (Input.anyKeyDown)
            {
                if (_isAbsence)
                {
                    _isAbsence = false;
                    UserReturnEvent?.Invoke(_absenceSecond);
                }

                _absenceSecond = 0;
            }

            _absenceSecond += Time.deltaTime;
            if (_absenceSecond >= NEEDED_MIN * 60 && !_isAbsence)
            {
                _isAbsence = true;
                InvokeInteractionEvent();
            }
        }
    }
}