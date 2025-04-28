using Code.Data;
using Code.Game.Services.Interactions;
using Code.Game.Services.Time;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.Entities.Diva.Reactions
{
    [Preserve]
    public class BadInteractionReaction: Reaction, ISubscriber
    {
        private DivaMaterialAdapter _characterMaterialAdapter;
        private InteractionStorage _interactionStorage;

        private int _cooldownMinutes;

        protected override UniTask InitializeReaction()
        {
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterMaterialAdapter = diva.FindCommonComponent<DivaMaterialAdapter>();
            
            _cooldownMinutes = Container.Instance.GetConfig<TimeConfig>().Cooldown.BadInteractionReactionMin;
            
            return base.InitializeReaction();
        }

        protected override int GetCooldownMinutes()
        {
            return _cooldownMinutes;
        }
        
        public void Subscribe()
        {
            _interactionStorage.OnAdded += _onAddedInteraction;
        }

        public void Unsubscribe()
        {
            _interactionStorage.OnAdded -= _onAddedInteraction;
        }

        private void _onAddedInteraction(EInteractionType type, int arg2)
        {
            if(type == EInteractionType.Bad)
            {
                _characterMaterialAdapter.PlayDoodle();
            }
        }
    }
}