using Code.Components.Characters.Reactions;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAudioReaction : CharacterReaction
    {
        [Header("Components")] 
        [SerializeField] private CharacterAnimator _characterAnimator;
        protected override int _cooldownTickCount { get; set; }

        private RangedFloat _effectAwakeningValue;
        private LiveStateStorage _liveStateStorage;

        protected override void SetCooldownMinutes()
        {
            _cooldownTickCount = Container.Instance.FindConfig<TimeConfig>().Cooldown.ReactionMaxAudioClip;
            _effectAwakeningValue = Container.Instance.FindConfig<CharacterConfig>().EffectAwakeningValue;
            _liveStateStorage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        public override void StartReaction()
        {
            _characterAnimator.PlayReactionVoice();
            RemoveLiveStateValue();
            base.StartReaction();
            base.StopReaction();
        }

        private void RemoveLiveStateValue()
        {
            _liveStateStorage.AddPercentageValue(new LiveStateValue()
            {
                Key = LiveStateKey.Sleep,
                Value = _effectAwakeningValue.GetRandomValue()
            });
        }
    }
}