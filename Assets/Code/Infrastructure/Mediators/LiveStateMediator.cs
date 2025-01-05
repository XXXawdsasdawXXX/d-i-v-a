﻿using System;
using Code.Components.Entities;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;

namespace Code.Infrastructure.Mediators
{
    public class LiveStateMediator : IMono, IGameInitListener, IGameStartListener, IGameExitListener
    {
        private LiveStateStorage _liveStateStorage;

        private DivaItemsController _characterItemsController;
        private InteractionStorage _interactionStorage;

        public void GameInit()
        {
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();

            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _characterItemsController = Container.Instance.FindEntity<Diva>()
                .FindCharacterComponent<DivaItemsController>();
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
                _characterItemsController.OnItemUsed += OnItemUsed;
                _interactionStorage.OnAdd += OnAddInteraction;
            }
            else
            {
                _characterItemsController.OnItemUsed -= OnItemUsed;
                _interactionStorage.OnAdd -= OnAddInteraction;
            }
        }

        private void OnAddInteraction(EInteractionType interactionType, int value)
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

        private void OnItemUsed(LiveStatePercentageValue[] values)
        {
            _liveStateStorage.AddPercentageValues(values);
        }
    }
}