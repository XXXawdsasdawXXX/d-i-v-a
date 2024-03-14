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
    public class InteractionObserver : IService, IGameInitListener, IGameExitListener
    {
        [Header("Observer components")]
        private ColliderButton _characterCollisionButton;
        
        [Header("Static data")]
        private InteractionStorage _interactionStorage;


        public void GameInit()
        {		
			//Observer components
            var diva = Container.Instance.FindEntity<DIVA>();
            _characterCollisionButton = diva.FindCommonComponent<ColliderButton>();
			//Static data
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();         
   
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
                _characterCollisionButton.SeriesOfClicksEvent += OnClickSeries;
            }
            else
            {
                _characterCollisionButton.SeriesOfClicksEvent -= OnClickSeries;
            }
        }

        private void OnClickSeries(int click)
        {
            if(click == 1)
            {
                Debugging.Instance.Log($"[service] click good series to character {click}",Debugging.Type.Interaction);
                _interactionStorage.Add(InteractionType.Good);
            }
            else if (click < 3)
            {
                Debugging.Instance.Log($"[service] click series to character {click}",Debugging.Type.Interaction);
                  _interactionStorage.Add(InteractionType.Normal);
            }
            else
            {
                
                Debugging.Instance.Log($"[service] click bad series to character {click}",Debugging.Type.Interaction); 
                _interactionStorage.Add(InteractionType.Bad);
            }
        }

    }
}