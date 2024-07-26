using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Objects;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Services
{
    public class InteractionObserver_ClickSeries : InteractionObserver, IGameInitListener, IGameStartListener,IGameExitListener
    {
        [Header("Observer components")] 
        private ColliderButton _characterCollisionButton;
        private CharacterLiveStatesAnalytic _characterState;

        [Header("Static data")] 
        private InteractionStorage _interactionStorage;

        public InteractionObserver_ClickSeries()
        {
            Debugging.Instance.Log($"construct click series",Debugging.Type.Interaction);
        }

        public void GameInit()
        {
            //Observer components
            var diva = Container.Instance.FindEntity<DIVA>();
            _characterCollisionButton = diva.FindCommonComponent<ColliderButton>();
            _characterState = diva.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            //Static data
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            SubscribeToEvents(true);
                Debugging.Instance.Log($"[ClickSeries] init", Debugging.Type.Interaction);
        }

        public void GameStart()
        {
            
                Debugging.Instance.Log($"[ClickSeries] start", Debugging.Type.Interaction);
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
                Debugging.Instance.Log($"[ClickSeries]subscribe {_characterCollisionButton != null}", Debugging.Type.Interaction);
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
                Debugging.Instance.Log($"[ClickSeries] click good series to character {click}", Debugging.Type.Interaction);
                _interactionStorage.Add(InteractionType.Good);
                InvokeInteractionEvent();
            }
            else if (click < 3)
            {
                if (_characterState.TryGetLowerSate(out var lowerKey, out var percent) &&
                    lowerKey == LiveStateKey.Sleep)
                {
                    _interactionStorage.Add(InteractionType.Bad);
                Debugging.Instance.Log($"[ClickSeries] click bad series to character {click}", Debugging.Type.Interaction);
                }
                else
                {
                    _interactionStorage.Add(InteractionType.Normal);
                Debugging.Instance.Log($"[ClickSeries] click series to character {click}", Debugging.Type.Interaction);
                }

                InvokeInteractionEvent();
            }
            else
            {
                Debugging.Instance.Log($"[ClickSeries] click bad series to character {click}", Debugging.Type.Interaction);
                _interactionStorage.Add(InteractionType.Bad);
                InvokeInteractionEvent();
            }
        }
    }
}