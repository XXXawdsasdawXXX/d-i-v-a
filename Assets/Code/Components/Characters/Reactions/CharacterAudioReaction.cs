using Code.Components.Characters.Reactions;
using Code.Data.Configs;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAudioReaction : CharacterReaction
    {
        [Header("Components")] 
        [SerializeField] private CharacterAnimator _characterAnimator;


        protected override float _maxCooldownMinutes { get; set; }

        protected override void SetCooldownMinutes()
        {
            _maxCooldownMinutes = Container.Instance.FindConfig<CharacterConfig>().Cooldowns.ReactionMaxAudioClip;
        }

        public override void StartReaction()
        {
            _characterAnimator.PlayReactionVoice();
            base.StartReaction();
            base.StopReaction();
        }
    }
}