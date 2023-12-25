using System;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _characterAnimator;

        private readonly int _standHash = Animator.StringToHash("Stand");
        private readonly int _seatHash = Animator.StringToHash("Seat");
        private readonly int _sleepHash = Animator.StringToHash("Sleep");

        private readonly int _reactionVoiceHash = Animator.StringToHash("ReactionVoice");
        public event Action<CharacterAnimationMode> ModeEnteredEvent;
        public CharacterAnimationMode Mode { get; private set; }


        #region Reaction Animation

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash);
        }

        #endregion

        #region SetMode

        public void SetSleepMode()
        {
            if (Mode == CharacterAnimationMode.Sleep)
            {
                Debugging.Instance?.Log($"Animation set mode {Mode} -> return", Debugging.Type.AnimationMode);
                return;
            }
            _characterAnimator.SetTrigger(_sleepHash);
            Mode = CharacterAnimationMode.Sleep;
            Debugging.Instance?.Log($"Animation set mode {Mode}", Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);
        }

        public void SetStandMode()
        {
            if (Mode == CharacterAnimationMode.Stand)
            {
                Debugging.Instance?.Log($"Animation set mode {Mode} -> return", Debugging.Type.AnimationMode);
                return;
            }

            _characterAnimator.SetTrigger(_standHash);
            Mode = CharacterAnimationMode.Stand;
            Debugging.Instance?.Log($"Animation set mode {Mode}", Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);
        }

        public void SetSeatMode()
        {
            if (Mode == CharacterAnimationMode.Seat)
            {
                Debugging.Instance?.Log($"Animation set mode {Mode} -> return", Debugging.Type.AnimationMode);
                return;
            }

            _characterAnimator.SetTrigger(_seatHash);
            Mode = CharacterAnimationMode.Seat;
            Debugging.Instance?.Log($"Animation set mode {Mode}", Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);
        }


        public void EnterToMode(CharacterAnimationMode state)
        {
            switch (state)
            {
                default:
                case CharacterAnimationMode.None:
                case CharacterAnimationMode.Stand:
                    SetStandMode();
                    break;
                case CharacterAnimationMode.Seat:
                    SetSeatMode();
                    break;
                case CharacterAnimationMode.Sleep:
                    SetSleepMode();
                    break;
            }
        }

        public void EnterToMode(LiveStateKey state)
        {
            switch (state)
            {
                case LiveStateKey.None:
                    SetStandMode();
                    break;
                case LiveStateKey.Sleep:
                    SetSleepMode();
                    break;
                case LiveStateKey.Hunger:
                    SetSeatMode();
                    break;
                case LiveStateKey.Fear:
                    SetSeatMode();
                    break;
                default:
                    SetStandMode();
                    break;
            }
        }

        #endregion
    }
}