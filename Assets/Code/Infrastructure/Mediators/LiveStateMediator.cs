using System;
using Code.Components.Characters;
using Code.Components.Entities;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;

namespace Code.Infrastructure.Mediators
{
    public class LiveStateMediator: IMono, IGameInitListener, IGameStartListener,IGameExitListener
    {
        private LiveStateStorage _liveStateStorage;
        
        private CharacterItemsController _characterItemsController;
        private InteractionStorage _interactionStorage;

        public void GameInit()
        {
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _characterItemsController = Container.Instance.FindEntity<DIVA>()
                .FindCharacterComponent<CharacterItemsController>();
        }

        public void GameStart()
        {
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
                _characterItemsController.OnUseItem += OnUseItem;
                _interactionStorage.OnAdd += OnAddInteraction;
            }
            else
            {
                _characterItemsController.OnUseItem -= OnUseItem;
                _interactionStorage.OnAdd -= OnAddInteraction;
            }
        }

        private void OnAddInteraction(InteractionType interactionType, float value)
        {
            switch (interactionType)
            {
                case InteractionType.None:
                default:
                    break;
                case InteractionType.Good:
                case InteractionType.Normal:
                    _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = LiveStateKey.Trust,
                        Value = value
                    });
                    break;
                case InteractionType.Bad:
                    _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = LiveStateKey.Trust,
                        Value = -value
                    });
                    break;
            }        
        }

        private void OnUseItem(LiveStatePercentageValue[] values)
        {
            _liveStateStorage.AddPercentageValues(values);
        }
    }
}