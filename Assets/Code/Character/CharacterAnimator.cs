using System;
using Code.Character.AnimationReader.Mode;
using Code.Character.AnimationReader.State;
using Code.Utils;
using UnityEngine;

namespace Code.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _characterAnimator;
        
        private readonly int _standHash = Animator.StringToHash("Stand");
        private readonly int _seatHash = Animator.StringToHash("Seat");

        private readonly int _reactionVoiceHash = Animator.StringToHash("ReactionVoice");
        
        public event Action<CharacterAnimationMode> ModeEnteredEvent;


        public CharacterAnimationMode Mode { get; private set; }

        private void Awake()
        {
            
        }

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash);
        }

        public void PlayStand()
        {
            _characterAnimator.SetTrigger(_standHash);
            Mode = CharacterAnimationMode.Stand;
            Debugging.Instance?.Log($"Animation set mode {Mode}",Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);            
        }

        public void PlaySeat()
        {
            _characterAnimator.SetTrigger(_seatHash);
            Mode = CharacterAnimationMode.Seat;
            Debugging.Instance?.Log($"Animation set mode {Mode}",Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);            
        }
    }
}