using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Interactions;

namespace Code.Infrastructure.Reactions
{
    public class InputWordReaction : Reaction, IGameStartListener, IGameExitListener
    {
        private Interaction_KeyDown _interactionKeyDown;
        private AudioEventsService _audioEventServices;
        private DivaAnimationAnalytic _animationAnalytic;

        private InputWord _lastWord;

        protected override void Init()
        {
            _interactionKeyDown = Container.Instance.FindInteractionObserver<Interaction_KeyDown>();
            _audioEventServices = Container.Instance.FindService<AudioEventsService>();
            _animationAnalytic = Container.Instance.FindEntity<Diva>()
                .FindCharacterComponent<DivaAnimationAnalytic>();
        }

        public void GameStart()
        {
            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        protected override int GetCooldownMinutes()
        {
            return Container.Instance.FindConfig<TimeConfig>().Cooldown.InputWordsReactionMin;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _interactionKeyDown.OnWordEntered += InteractionKeyDownOnOnWordEntered;
            }
            else
            {
                _interactionKeyDown.OnWordEntered -= InteractionKeyDownOnOnWordEntered;
            }
        }

        private void InteractionKeyDownOnOnWordEntered(InputWord word)
        {
            if (!IsReady() || _animationAnalytic.CurrentMode is CharacterAnimationMode.Sleep)
            {
                return;
            }
            
            _lastWord = word;
            StartReaction();
        }

        public override void StartReaction()
        {
            switch (_lastWord)
            {
                case InputWord.hello:
                case InputWord.hi:
                case InputWord.ghbdtn:
                case InputWord.yo:
                    _audioEventServices.PlayAudio(AudioEventType.Hi);
                    break;
                case InputWord.love:
                    _audioEventServices.PlayAudio(AudioEventType.Song);
                    break;
                default:
                    break;
            }

            base.StartReaction();
            StopReaction();
        }
    }
}