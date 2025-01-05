using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class InteractionObserver_ClickOnCharacter : InteractionObserver, IGameInitListener, IGameExitListener
    {
        [Header("Observer components")] 
        private ColliderButton _characterCollisionButton;
        private DivaLiveStatesAnalytic _divaState;

        [Header("Static data")] 
        private InteractionStorage _interactionStorage;

        
        public InteractionObserver_ClickOnCharacter()
        {
            Debugging.Instance.Log($"construct click series", Debugging.Type.Interaction);
        }

        public void GameInit()
        {
            //Observer components
            Diva diva = Container.Instance.FindEntity<Diva>();
            _characterCollisionButton = diva.FindCommonComponent<ColliderButton>();
            _divaState = diva.FindCharacterComponent<DivaLiveStatesAnalytic>();

            //Static data
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            SubscribeToEvents(true);

            Debugging.Instance.Log($"[ClickSeries] init", Debugging.Type.Interaction);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterCollisionButton.SeriesOfClicksEvent += OnClickSeries;
            }
            else
            {
                _characterCollisionButton.SeriesOfClicksEvent -= OnClickSeries;
            }
        }

        private void OnClickSeries(int click)
        {
            Debugging.Instance.Log($"[ClickSeries] ???????????? {click}", Debugging.Type.Interaction);
            if (click == 1)
            {
                Debugging.Instance.Log($"[ClickSeries] click good series to character {click}",
                    Debugging.Type.Interaction);
                _interactionStorage.Add(InteractionType.Good);
                InvokeInteractionEvent();
            }
            else if (click < 3)
            {
                if (_divaState.TryGetLowerSate(out ELiveStateKey lowerKey, out float percent) &&
                    lowerKey == ELiveStateKey.Sleep)
                {
                    _interactionStorage.Add(InteractionType.Bad);
                    Debugging.Instance.Log($"[ClickSeries] click bad series to character {click}",
                        Debugging.Type.Interaction);
                }
                else
                {
                    _interactionStorage.Add(InteractionType.Normal);
                    Debugging.Instance.Log($"[ClickSeries] click series to character {click}",
                        Debugging.Type.Interaction);
                }

                InvokeInteractionEvent();
            }
            else
            {
                Debugging.Instance.Log($"[ClickSeries] click bad series to character {click}",
                    Debugging.Type.Interaction);
                _interactionStorage.Add(InteractionType.Bad);
                InvokeInteractionEvent();
            }
        }
    }
}