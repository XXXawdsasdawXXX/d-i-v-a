using Code.Data;
using Code.Entities.Common;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Entities.Hand
{
    public class HandMaterialAdapter : MaterialAdapter, IInitListener, ISubscriber
    {
        [Header("Components")] 
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Static values")] 
        private HandConfig _handConfig;
        private InteractionStorage _interactionStorage;

        public UniTask GameInitialize()
        {
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            return UniTask.CompletedTask;
        }
        
        public UniTask Subscribe()
        {
            _interactionStorage.OnSwitchDominationType += _onSwitchDominationInteraction;
            
            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _interactionStorage.OnSwitchDominationType -= _onSwitchDominationInteraction;
        }

        private void _onSwitchDominationInteraction(EInteractionType interactionType)
        {
            switch (interactionType)
            {
                default:
                case EInteractionType.None:
                case EInteractionType.Good:
                case EInteractionType.Normal:
                    _setLightMaterial();
                    break;
              
                case EInteractionType.Bad:
                    _setDarkMaterial();
                    break;
            }
        }

        private void _setLightMaterial()
        {
            _material = _handConfig.LightMaterial;
            _spriteRenderer.material = _material;
        }

        private void _setDarkMaterial()
        {
            _material = _handConfig.DarkMaterial;
            _spriteRenderer.material = _material;
        }
    }
}