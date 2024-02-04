using Code.Components.Characters.Reactions;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAudioReaction : CharacterReaction
    {
        [Header("Components")] 
        [SerializeField] private CharacterAnimator _characterAnimator;
        
        public override void StartReaction()
        {
            _characterAnimator.PlayReactionVoice();
            base.StartReaction();
            base.StopReaction();
        }
    }
}