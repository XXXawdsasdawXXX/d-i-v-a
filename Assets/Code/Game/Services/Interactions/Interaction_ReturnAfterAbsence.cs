using System;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.Services.Interactions
{
    [Preserve]
    public class Interaction_ReturnAfterAbsence : InteractionObserver, IInitializeListener, IUpdateListener
    {
        public event Action<float> UserReturnEvent;
        
        public bool IsAbsence { get; private set; }
        
        private float _userStandStillSecond;
        private float _absenceSecond;

        public UniTask GameInitialize()
        {
            _userStandStillSecond = Container.Instance.GetConfig<TimeConfig>().Duration.UserStandStillSecond;
            
            return UniTask.CompletedTask;
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

            _absenceSecond += UnityEngine.Time.deltaTime;
        
            if (_absenceSecond >= _userStandStillSecond * 60 && !IsAbsence)
            {
                IsAbsence = true;
              
                InvokeInteractionEvent();
            }
        }
    }
}