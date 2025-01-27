using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Entities.Diva.Reactions
{
    [Preserve]
    public class AudioReaction : Reaction, ISubscriber
    {
        public event Action EndReactionEvent;
        
        [Header("Components")]
        private DivaAnimator _divaAnimator;
        private DivaAnimationStateObserver _stateReader;
        
        [Header("Services")]
        private LiveStateStorage _liveStateStorage;
        
        [Header("Values")]
        private LiveStateRangePercentageValue _effectAwakeningValue;
        private int _cooldownMinutes;

        protected override UniTask InitializeReaction()
        {
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _stateReader = diva.FindCharacterComponent<DivaAnimationStateObserver>();
            
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
            _effectAwakeningValue = Container.Instance.FindConfig<LiveStateConfig>().Awakening;
            
            _cooldownMinutes = Container.Instance.FindConfig<TimeConfig>().Cooldown.MaxAudioClipReactionMin;
            
            return base.InitializeReaction();
        }

        public UniTask Subscribe()
        {
            _stateReader.OnStateExited += _onAnimationOnStateExited;
            
            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _stateReader.OnStateExited -= _onAnimationOnStateExited;
        }

        public override void StartReaction()
        {
            _divaAnimator.PlayReactionVoice();
            
            _removeLiveStateValue();
            
            base.StartReaction();
            
            base.StopReaction();
        }

        protected override int GetCooldownMinutes()
        {
            return _cooldownMinutes;
        }

        private void _removeLiveStateValue()
        {
            _liveStateStorage.AddPercentageValue(_effectAwakeningValue);
        }

        private void _onAnimationOnStateExited(EDivaAnimationState obj)
        {
            if (obj == EDivaAnimationState.ReactionVoice)
            {
                EndReactionEvent?.Invoke();
            }
        }
    }
}