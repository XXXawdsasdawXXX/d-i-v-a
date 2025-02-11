﻿using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Services.Interactions
{
    [Preserve]
    public class InteractionObserver_ClickOnCharacter : InteractionObserver, IInitListener, ISubscriber
    {
        [Header("Observer components")] 
        private ColliderButton _characterCollisionButton;
        private DivaLiveStatesAnalytic _divaState;

        [Header("Static data")] 
        private InteractionStorage _interactionStorage;

        public UniTask GameInitialize()
        {
            //Observer components
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterCollisionButton = diva.FindCommonComponent<ColliderButton>();
            _divaState = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();

            //Static data
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
#if DEBUGGING
            Debugging.Log(this, "[GameInit]", Debugging.Type.Interaction);
#endif
            return UniTask.CompletedTask;
        }

        public UniTask Subscribe()
        {
            _characterCollisionButton.SeriesOfClicksEvent += _onClickSeries;
            
            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _characterCollisionButton.SeriesOfClicksEvent -= _onClickSeries;
        }
     

        private void _onClickSeries(int click)
        {
#if DEBUGGING
            Debugging.Log(this, $"[_onClickSeries] Click #{click}.", Debugging.Type.Interaction);
#endif
            if (click == 1)
            {
#if DEBUGGING
                Debugging.Log(this, "[_onClickSeries] Click good series.", Debugging.Type.Interaction);
#endif
                _interactionStorage.Add(EInteractionType.Good);
            
                InvokeInteractionEvent();
            }
            else if (click < 3)
            {
                if (_divaState.TryGetLowerSate(out ELiveStateKey lowerKey, out float _) && lowerKey == ELiveStateKey.Sleep)
                {
                    _interactionStorage.Add(EInteractionType.Bad);
#if DEBUGGING
                    Debugging.Log(this, "[_onClickSeries] Click bad series.", Debugging.Type.Interaction);
#endif
                }
                else
                {
                    _interactionStorage.Add(EInteractionType.Normal);
#if DEBUGGING
                    Debugging.Log(this, "[_onClickSeries] Click normal series", Debugging.Type.Interaction);
#endif
                }

                InvokeInteractionEvent();
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, "[_onClickSeries] Click bad series.", Debugging.Type.Interaction);
#endif
                _interactionStorage.Add(EInteractionType.Bad);
            
                InvokeInteractionEvent();
            }
        }

    }
}