using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;

namespace Code.Components.Entities.Characters.Reactions
{
    public class BadInteractionReaction: Reaction, IGameStartListener, IGameExitListener
    {
        private CharacterMaterialAdapter _characterMaterialAdapter;
        private InteractionStorage _interactionStorage;

        protected override void Init()
        {
            var diva = Container.Instance.FindEntity<DIVA>();
            _characterMaterialAdapter = diva.FindCommonComponent<CharacterMaterialAdapter>();

            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            
            base.Init();
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.BadInteractionReaction;
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

        private void OnAddedInteraction(InteractionType type, int arg2)
        {
            if(type == InteractionType.Bad)
            {
                _characterMaterialAdapter.PlayDoodle();
            }
        }
    }
}