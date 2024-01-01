﻿using System;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _characterAnimator;

        private readonly int _standHash_t = Animator.StringToHash("Stand");
        private readonly int _seatHash_t = Animator.StringToHash("Seat");
        private readonly int _sleepHash_t = Animator.StringToHash("Sleep");
        private readonly int _empty_b = Animator.StringToHash("Empty");

        private readonly int _eatHash_b = Animator.StringToHash("Eat");
        private readonly int _reactionVoiceHash_t = Animator.StringToHash("ReactionVoice");
        public event Action<CharacterAnimationMode> ModeEnteredEvent;
        public CharacterAnimationMode Mode { get; private set; }


        #region Reaction Animation

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash_t);
        }
        
        public void StartPlayEat()
        {
            _characterAnimator.SetBool(_eatHash_b, true);
        }

        public void StopPlayEat()
        {
            _characterAnimator.SetBool(_eatHash_b, false);
        }


        
        
        #endregion

        #region SetMode

        public void SetEmptyMode()
        {
            if (Mode == CharacterAnimationMode.None)
            {
                Debugging.Instance?.Log($"Animation set mode {Mode} -> return", Debugging.Type.AnimationMode);
                return;
            }
            _characterAnimator.SetBool(_empty_b, true);
            Mode = CharacterAnimationMode.None;
            Debugging.Instance?.Log($"Animation set mode {Mode}", Debugging.Type.AnimationMode);
            ModeEnteredEvent?.Invoke(Mode);
        }
        
        public void SetSleepMode()
        {
            if (Mode == CharacterAnimationMode.Sleep)
            {
                Debugging.Instance?.Log($"Animation set mode {Mode} -> return", Debugging.Type.AnimationMode);
                return;
            }

            ResetBoolStates();
            
            _characterAnimator.SetTrigger(_sleepHash_t);
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

            ResetBoolStates();


            _characterAnimator.SetTrigger(_standHash_t);
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
            ResetBoolStates();
     

            _characterAnimator.SetTrigger(_seatHash_t);
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
                    SetEmptyMode();
                    break;
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
                    SetEmptyMode();
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

        private void ResetBoolStates()
        {
            if (Mode == CharacterAnimationMode.None)
            {
                _characterAnimator.SetBool(_empty_b, false);
            }

            _characterAnimator.SetBool(_eatHash_b, false);
        }
    }
}