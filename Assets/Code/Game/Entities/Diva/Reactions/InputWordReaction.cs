using Code.Game.Services.Interactions;
using Code.Game.Services.Time;
using Code.Infrastructure.Audio;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;

namespace Code.Game.Entities.Diva.Reactions
{
    [Preserve]
    public class InputWordReaction : Reaction, ISubscriber
    {
        private Interaction_KeyDown _interactionKeyDown;
        private AudioEventsService _audioEventServices;
        private DivaAnimationAnalytic _animationAnalytic;

        private EInputWord _lastWord;

        protected override UniTask InitializeReaction()
        {
            _interactionKeyDown = Container.Instance.FindInteractionObserver<Interaction_KeyDown>();
            _audioEventServices = Container.Instance.GetService<AudioEventsService>();
            
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();

            return base.InitializeReaction();
        }
        
        public void Subscribe()
        {
            _interactionKeyDown.OnWorldEntered += _onWorldEntered;
        }

        public void Unsubscribe()
        {
            _interactionKeyDown.OnWorldEntered -= _onWorldEntered;
        }
        
        protected override int GetCooldownMinutes()
        {
            return Container.Instance.GetConfig<TimeConfig>().Cooldown.InputWordsReactionMin;
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