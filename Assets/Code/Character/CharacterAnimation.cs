using UnityEngine;

namespace Code.Character
{
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _characterAnimator;
        private readonly int _reactionVoice = Animator.StringToHash("ReactionVoice");

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoice);
        }
        
        
    }
}