﻿using Code.Data;
using Code.Game.Entities.Diva;
using Code.Game.Services.Interactions;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.Services.LiveState
{
    [Preserve]
    public class LiveStateMediator : IMono, IInitializeListener, ISubscriber
    {
        private LiveStateStorage _liveStateStorage;
        private DivaItemsController _characterItemsController;
        private InteractionStorage _interactionStorage;

        public UniTask GameInitialize()
        {
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterItemsController = diva.FindCharacterComponent<DivaItemsController>();
            
            return UniTask.CompletedTask;
        }

        public void Subscribe()
        {
            _characterItemsController.OnItemUsed += _onItemUsed;
            _interactionStorage.OnAdded += _onAddedInteraction;
        }

        public void Unsubscribe()
        {
            _characterItemsController.OnItemUsed -= _onItemUsed;
            _interactionStorage.OnAdded -= _onAddedInteraction;
        }

        private void _onAddedInteraction(EInteractionType interactionType, int value)
        {
            switch (interactionType)
            {
                case EInteractionType.None:
                default:
                    break;
               
                case EInteractionType.Good:
                case EInteractionType.Normal:
                    _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = ELiveStateKey.Trust,
                        Value = value
                    });
                    break;
             
                case EInteractionType.Bad:
                    _liveStateStorage.AddPercentageValue(new LiveStatePercentageValue()
                    {
                        Key = ELiveStateKey.Trust,
                        Value = -value
                    });
                    break;
            }
        }

        private void _onItemUsed(LiveStatePercentageValue[] values)
        {
            _liveStateStorage.AddPercentageValues(values);
        }
    }
}