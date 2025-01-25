using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Interactions;

namespace Code.Entities.Diva.Reactions
{
    public class InputWordReaction : Reaction, IStartListener, IExitListener
    {
        private Interaction_KeyDown _interactionKeyDown;
        private AudioEventsService _audioEventServices;
        private DivaAnimationAnalytic _animationAnalytic;

        private EInputWord _lastWord;

        protected override void Init()
        {
            _interactionKeyDown = Container.Instance.FindInteractionObserver<Interaction_KeyDown>();
            _audioEventServices = Container.Instance.FindService<AudioEventsService>();
            _animationAnalytic = Container.Instance.FindEntity<DivaEntity>()
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
                _interactionKeyDown.OnWorldEntered += _onWorldEntered;
            }
            else
            {
                _interactionKeyDown.OnWorldEntered -= _onWorldEntered;
            }
        }

        private void _onWorldEntered(EInputWord word)
        {
            if (!IsReady() || _animationAnalytic.CurrentMode is EDivaAnimationMode.Sleep)
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
                case EInputWord.hello:
                case EInputWord.hi:
                case EInputWord.ghbdtn:
                case EInputWord.yo:
                    _audioEventServices.PlayAudio(EAudioEventType.Hi);
                    break;
                case EInputWord.love:
                    _audioEventServices.PlayAudio(EAudioEventType.Song);
                    break;
                default:
                    break;
            }

            base.StartReaction();
            StopReaction();
        }
    }
}