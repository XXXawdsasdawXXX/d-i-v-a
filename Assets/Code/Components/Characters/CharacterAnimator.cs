using System;
using System.Collections;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAnimator : CharacterComponent, IGameInitListener
    {
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private Animator _frontHairAnimator;
        [SerializeField] private Animator _backHairAnimator;

        private readonly int _standHash_t = Animator.StringToHash("Stand");
        private readonly int _seatHash_t = Animator.StringToHash("Seat");
        private readonly int _sleepHash_t = Animator.StringToHash("Sleep");
        private readonly int _empty_b = Animator.StringToHash("Empty");

        private readonly int _eatHash_b = Animator.StringToHash("Eat");
        private readonly int _reactionVoiceHash_t = Animator.StringToHash("ReactionVoice");
        private readonly int _reactionMouseHash_b = Animator.StringToHash("ReactionMouse");
        
        private readonly int _mouseXHash_f = Animator.StringToHash("MouseX");
        private readonly int _mouseYHash_f = Animator.StringToHash("MouseY");

        public CharacterAnimationMode Mode { get; private set; }

        private CoroutineRunner _coroutineRunner;
        public event Action<CharacterAnimationMode> ModeEnteredEvent;//EVENT!!^_^
        
        public void GameInit()
        {
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
        }

        #region Reaction Animation

        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash_t);
            _frontHairAnimator.SetTrigger(_reactionVoiceHash_t);
            _backHairAnimator.SetTrigger(_reactionVoiceHash_t);
        }

        public void StartPlayEat(Action OnReadyEat = null)
        {
            _characterAnimator.SetBool(_eatHash_b, true);
            _frontHairAnimator.SetBool(_eatHash_b, true);
            _backHairAnimator.SetBool(_eatHash_b, true);
            _coroutineRunner.StartActionWithDelay(OnReadyEat, 1);
        }

        public void StopPlayEat()
        {
            _characterAnimator.SetBool(_eatHash_b, false);
            _frontHairAnimator.SetBool(_eatHash_b, false);
            _backHairAnimator.SetBool(_eatHash_b, false);
        }


        public void StartPlayReactionMouse()
        {
            _characterAnimator.SetBool(_reactionMouseHash_b, true);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, true);
            _backHairAnimator.SetBool(_reactionMouseHash_b, true);
            Debugging.Instance.Log($"Start play reaction mouse", Debugging.Type.AnimationState);
        }

        public void StopPlayReactionMouse()
        {
            _characterAnimator.SetBool(_reactionMouseHash_b, false);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, false);
            _backHairAnimator.SetBool(_reactionMouseHash_b, false);
            Debugging.Instance.Log($"Stop play reaction mouse", Debugging.Type.AnimationState);
        }


        public void SetMouseNormal(float x, float y)
        {
            _characterAnimator.SetFloat(_mouseXHash_f,x);
            _characterAnimator.SetFloat(_mouseYHash_f,y);
            
            _frontHairAnimator.SetFloat(_mouseXHash_f,x);
            _frontHairAnimator.SetFloat(_mouseYHash_f,y);
            
            _backHairAnimator.SetFloat(_mouseXHash_f,x);
            _backHairAnimator.SetFloat(_mouseYHash_f,y);
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
            _frontHairAnimator.SetBool(_empty_b, true);
            _backHairAnimator.SetBool(_empty_b, true);

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

            Reset();

            _characterAnimator.SetTrigger(_sleepHash_t);
            _frontHairAnimator.SetTrigger(_sleepHash_t);
            _backHairAnimator.SetTrigger(_sleepHash_t);

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

            Reset();

            _characterAnimator.SetTrigger(_standHash_t);
            _frontHairAnimator.SetTrigger(_standHash_t);
            _backHairAnimator.SetTrigger(_standHash_t);

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

            Reset();

            _characterAnimator.SetTrigger(_seatHash_t);
            _frontHairAnimator.SetTrigger(_seatHash_t);
            _backHairAnimator.SetTrigger(_seatHash_t);

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

        #endregion

        private void Reset()
        {
            ResetBoolStates();
            ResetTriggers();
        }

        private void ResetBoolStates()
        {
            if (Mode == CharacterAnimationMode.None)
            {
                _characterAnimator.SetBool(_empty_b, false);
                _frontHairAnimator.SetBool(_empty_b, false);
                _backHairAnimator.SetBool(_empty_b, false);
            }

            _characterAnimator.SetBool(_eatHash_b, false);
            _frontHairAnimator.SetBool(_eatHash_b, false);
            _backHairAnimator.SetBool(_eatHash_b, false);
        }

        private void ResetTriggers()
        {
            _characterAnimator.ResetTrigger(_reactionVoiceHash_t);
            _frontHairAnimator.ResetTrigger(_reactionVoiceHash_t);
            _backHairAnimator.ResetTrigger(_reactionVoiceHash_t);
        }
    }
}