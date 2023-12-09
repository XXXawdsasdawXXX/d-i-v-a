using UnityEngine;

namespace Code.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _characterAnimator;
        
        private readonly int _standHash = Animator.StringToHash("Stand");
        private readonly int _seatHash = Animator.StringToHash("Seat");

        private readonly int _reactionVoiceHash = Animator.StringToHash("ReactionVoice");

        public CharacterAnimationMode AnimationMode { get; private set; }

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash);
        }

        public void PlayStand()
        {
            _characterAnimator.SetTrigger(_standHash);
            AnimationMode = CharacterAnimationMode.Stand;
            Debug.Log($"Mode {AnimationMode}");
        }

        public void PlaySeat()
        {
            _characterAnimator.SetTrigger(_seatHash);
            AnimationMode = CharacterAnimationMode.Seat;
            Debug.Log($"Mode {AnimationMode}");
        }
    }
}