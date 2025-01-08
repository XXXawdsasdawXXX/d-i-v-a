using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;

namespace Code.Infrastructure.Reactions
{
    public class BadInteractionReaction: Reaction, IGameStartListener, IGameExitListener
    {
        private DivaMaterialAdapter _characterMaterialAdapter;
        private InteractionStorage _interactionStorage;

        protected override void Init()
        {
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _characterMaterialAdapter = diva.FindCommonComponent<DivaMaterialAdapter>();

            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            base.Init();
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.BadInteractionReactionMin;
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
                _interactionStorage.OnAdd += OnAddedInteraction; 
            }
            else
            {
                _interactionStorage.OnAdd += OnAddedInteraction;
            }
        }

        private void OnAddedInteraction(EInteractionType type, int arg2)
        {
            if(type == EInteractionType.Bad)
            {
                _characterMaterialAdapter.PlayDoodle();
            }
        }
    }
}