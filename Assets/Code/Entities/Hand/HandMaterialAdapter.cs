using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Entities.Common;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Entities.Hand
{
    public class HandMaterialAdapter : MaterialAdapter, IGameInitListener, IGameExitListener
    {
        [Header("Components")] [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [Header("Static values")] private HandConfig _handConfig;
        private InteractionStorage _interactionStorage;

        public void GameInit()
        {
            _handConfig = Container.Instance.FindConfig<HandConfig>();
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
                _interactionStorage.OnSwitchDominationType += InteractionStorageOnSwitchDominationTypeEvent;
            }
            else
            {
                _interactionStorage.OnSwitchDominationType -= InteractionStorageOnSwitchDominationTypeEvent;
            }
        }

        private void SetLightMaterial()
        {
            _material = _handConfig.LightMaterial;
            _spriteRenderer.material = _material;
        }

        private void SetDarkMaterial()
        {
            _material = _handConfig.DarkMaterial;
            _spriteRenderer.material = _material;
        }

        private void InteractionStorageOnSwitchDominationTypeEvent(EInteractionType interactionType)
        {
            switch (interactionType)
            {
                default:
                case EInteractionType.None:
                case EInteractionType.Good:
                case EInteractionType.Normal:
                    SetLightMaterial();
                    break;
                case EInteractionType.Bad:
                    SetDarkMaterial();
                    break;
            }
        }
    }
}