using System;
using System.Collections;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Unity.VisualScripting;
using UnityEngine;
using CoroutineRunner = Code.Infrastructure.Services.CoroutineRunner;

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
        public event Action<CharacterAnimationMode> OnModeEntered;
        
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
            Debugging.Instance?.Log(this,$"PlayReactionVoice",Debugging.Type.AnimationState );
        }

        public void StartPlayEat(Action OnReadyEat = null)
        {
            _characterAnimator.SetBool(_eatHash_b, true);
            _frontHairAnimator.SetBool(_eatHash_b, true);
            _backHairAnimator.SetBool(_eatHash_b, true);
            _coroutineRunner.StartActionWithDelay(OnReadyEat, 1);
            Debugging.Instance?.Log(this,$"StartPlayEat",Debugging.Type.AnimationState );
        }

        public void StopPlayEat()
        {
            _characterAnimator.SetBool(_eatHash_b, false);
            _frontHairAnimator.SetBool(_eatHash_b, false);
            _backHairAnimator.SetBool(_eatHash_b, false);
            Debugging.Instance?.Log(this,$"StopPlayEat",Debugging.Type.AnimationState );
        }


        public void StartPlayReactionMouse()
        {
            /*_characterAnimator.SetBool(_reactionMouseHash_b, true);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, true);
            _backHairAnimator.SetBool(_reactionMouseHash_b, true);
            Debugging.Instance.Log($"Start play reaction mouse", Debugging.Type.AnimationState);*/
            Debugging.Instance?.Log(this,$"StartPlayReactionMouse",Debugging.Type.AnimationState );
        }

        public void StopPlayReactionMouse()
        {
            _characterAnimator.SetBool(_reactionMouseHash_b, false);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, false);
            _backHairAnimator.SetBool(_reactionMouseHash_b, false);
            Debugging.Instance?.Log(this,$"StopPlayReactionMouse",Debugging.Type.AnimationState );
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
                Debugging.Instance?.Log(this,$"Ready SetEmptyMode",Debugging.Type.AnimationMode );
                return;
            }

            _characterAnimator.SetBool(_empty_b, true);
            _frontHairAnimator.SetBool(_empty_b, true);
            _backHairAnimator.SetBool(_empty_b, true);

            Mode = CharacterAnimationMode.None;
            OnModeEntered?.Invoke(Mode);
            Debugging.Instance?.Log(this,$"SetEmptyMode",Debugging.Type.AnimationMode );
        }

        public void SetSleepMode()
        {
            if (Mode == CharacterAnimationMode.Sleep)
            {
                Debugging.Instance?.Log(this,$"Ready SetSleepMode",Debugging.Type.AnimationMode );
                return;
            }

            Reset();

            _characterAnimator.SetTrigger(_sleepHash_t);
            _frontHairAnimator.SetTrigger(_sleepHash_t);
            _backHairAnimator.SetTrigger(_sleepHash_t);

            Mode = CharacterAnimationMode.Sleep;
            OnModeEntered?.Invoke(Mode);
            Debugging.Instance?.Log(this,$"SetSleepMode",Debugging.Type.AnimationMode );
        }

        public void SetStandMode()
        {
            if (Mode == CharacterAnimationMode.Stand)
            {
                Debugging.Instance?.Log(this,$"Ready SetStandMode",Debugging.Type.AnimationMode );
                return;
            }

            Reset();

            _characterAnimator.SetTrigger(_standHash_t);
            _frontHairAnimator.SetTrigger(_standHash_t);
            _backHairAnimator.SetTrigger(_standHash_t);

            Mode = CharacterAnimationMode.Stand;
            OnModeEntered?.Invoke(Mode);
            Debugging.Instance?.Log(this,$"SetStandMode",Debugging.Type.AnimationMode);
        }

        public void SetSeatMode()
        {
            if (Mode == CharacterAnimationMode.Seat)
            {
                Debugging.Instance?.Log(this,$"Ready SetSeatMode",Debugging.Type.AnimationMode );
                return;
            }

            Reset();

            _characterAnimator.SetTrigger(_seatHash_t);
            _frontHairAnimator.SetTrigger(_seatHash_t);
            _backHairAnimator.SetTrigger(_seatHash_t);

            Mode = CharacterAnimationMode.Seat;
            OnModeEntered?.Invoke(Mode);
            Debugging.Instance?.Log(this,$"SetSeatMode",Debugging.Type.AnimationMode );
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
            Debugging.Instance?.Log(this,$"Reset",Debugging.Type.AnimationMode );
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