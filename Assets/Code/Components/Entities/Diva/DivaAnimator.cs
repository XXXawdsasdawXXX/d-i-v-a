using System;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using CoroutineRunner = Code.Infrastructure.Services.CoroutineRunner;

namespace Code.Components.Entities
{
    public class DivaAnimator : DivaComponent, IGameInitListener
    {
        public event Action<EDivaAnimationMode> OnModeEntered;
        public EDivaAnimationMode Mode { get; private set; }

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

        private readonly int _hideHand_b = Animator.StringToHash("HideHand");
        
        private CoroutineRunner _coroutineRunner;
        private Debugging _debug;
        

        public void GameInit()
        {
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _debug = Debugging.Instance;
        }

        #region Reaction Animation
        
        public void PlayReactionVoice()
        {
            _characterAnimator.SetTrigger(_reactionVoiceHash_t);
            _frontHairAnimator.SetTrigger(_reactionVoiceHash_t);
            _backHairAnimator.SetTrigger(_reactionVoiceHash_t);
            Debugging.Log(this, $"[PlayReactionVoice]", Debugging.Type.AnimationState);
        }

        public void StartPlayEat(Action OnReadyEat = null)
        {
            _characterAnimator.SetBool(_eatHash_b, true);
            _frontHairAnimator.SetBool(_eatHash_b, true);
            _backHairAnimator.SetBool(_eatHash_b, true);
            _coroutineRunner.StartActionWithDelay(OnReadyEat, 1);
            Debugging.Log(this, $"", Debugging.Type.AnimationState);
        }

        public void StopPlayEat()
        {
            _characterAnimator.SetBool(_eatHash_b, false);
            _frontHairAnimator.SetBool(_eatHash_b, false);
            _backHairAnimator.SetBool(_eatHash_b, false);
            Debugging.Log(this, $"[StopPlayEat]", Debugging.Type.AnimationState);
        }
        
        public void StartPlayReactionMouse()
        {
            _characterAnimator.SetBool(_reactionMouseHash_b, true);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, true);
            _backHairAnimator.SetBool(_reactionMouseHash_b, true);
            Debugging.Log(this, $"[StartPlayReactionMouse]", Debugging.Type.AnimationState);
        }

        public void StopPlayReactionMouse()
        {
            _characterAnimator.SetBool(_reactionMouseHash_b, false);
            _frontHairAnimator.SetBool(_reactionMouseHash_b, false);
            _backHairAnimator.SetBool(_reactionMouseHash_b, false);
            
            Debugging.Log(this, $"[StopPlayReactionMouse]", Debugging.Type.AnimationState);
        }
        
        public void SetMouseNormal(float x, float y)
        {
            _characterAnimator.SetFloat(_mouseXHash_f, x);
            _characterAnimator.SetFloat(_mouseYHash_f, y);

            _frontHairAnimator.SetFloat(_mouseXHash_f, x);
            _frontHairAnimator.SetFloat(_mouseYHash_f, y);

            _backHairAnimator.SetFloat(_mouseXHash_f, x);
            _backHairAnimator.SetFloat(_mouseYHash_f, y);
        }

        public void StartPlayHideHand()
        {
            _characterAnimator.SetBool(_hideHand_b, true);
            _frontHairAnimator.SetBool(_hideHand_b, true);
            _backHairAnimator.SetBool(_hideHand_b, true);
            
            Debugging.Log(this, $"[StartPlayHideHand]", Debugging.Type.AnimationState);
        }

        public void StopPlayHideHand()
        {
            _characterAnimator.SetBool(_hideHand_b, false);
            _frontHairAnimator.SetBool(_hideHand_b, false);
            _backHairAnimator.SetBool(_hideHand_b, false);

            Debugging.Log(this, $"[StopPlayHideHand]", Debugging.Type.AnimationState);
        }

        #endregion

        #region SetMode

        public void SetEmptyMode()
        {
            if (Mode == EDivaAnimationMode.None)
            {
                Debugging.Log(this, $"[SetEmptyMode] State is already set.", Debugging.Type.AnimationMode);
              
                return;
            }

            _characterAnimator.SetBool(_empty_b, true);
            _frontHairAnimator.SetBool(_empty_b, true);
            _backHairAnimator.SetBool(_empty_b, true);

            Mode = EDivaAnimationMode.None;
            
            OnModeEntered?.Invoke(Mode);
            
            Debugging.Log(this, $"[SetEmptyMode]", Debugging.Type.AnimationMode);
        }

        public void SetSleepMode()
        {
            if (Mode == EDivaAnimationMode.Sleep)
            {
                Debugging.Log(this, $"[SetSleepMode] State is already set.", Debugging.Type.AnimationMode);
                
                return;
            }

            _reset();

            _characterAnimator.SetTrigger(_sleepHash_t);
            _frontHairAnimator.SetTrigger(_sleepHash_t);
            _backHairAnimator.SetTrigger(_sleepHash_t);

            Mode = EDivaAnimationMode.Sleep;
         
            OnModeEntered?.Invoke(Mode);
            
            Debugging.Log(this, $"[SetSleepMode]", Debugging.Type.AnimationMode);
        }

        public void SetStandMode()
        {
            if (Mode == EDivaAnimationMode.Stand)
            {
                Debugging.Log(this, $"[SetStandMode] State is already set.", Debugging.Type.AnimationMode);
                return;
            }

            _reset();

            _characterAnimator.SetTrigger(_standHash_t);
            _frontHairAnimator.SetTrigger(_standHash_t);
            _backHairAnimator.SetTrigger(_standHash_t);

            Mode = EDivaAnimationMode.Stand;
            
            OnModeEntered?.Invoke(Mode);
        
            Debugging.Log(this, $"[SetStandMode]", Debugging.Type.AnimationMode);
        }

        public void SetSeatMode()
        {
            if (Mode == EDivaAnimationMode.Seat)
            {
                Debugging.Log(this, $"[SetSeatMode] State is already set.", Debugging.Type.AnimationMode);
                return;
            }

            _reset();

            _characterAnimator.SetTrigger(_seatHash_t);
            _frontHairAnimator.SetTrigger(_seatHash_t);
            _backHairAnimator.SetTrigger(_seatHash_t);

            Mode = EDivaAnimationMode.Seat;
            
            OnModeEntered?.Invoke(Mode);
            
            Debugging.Log(this, $"[SetSeatMode]", Debugging.Type.AnimationMode);
        }


        public void EnterToMode(EDivaAnimationMode state)
        {
            switch (state)
            {
                default:
                case EDivaAnimationMode.None:
                    SetEmptyMode();
                    break;
                case EDivaAnimationMode.Stand:
                    SetStandMode();
                    break;
                case EDivaAnimationMode.Seat:
                    SetSeatMode();
                    break;
                case EDivaAnimationMode.Sleep:
                    SetSleepMode();
                    break;
            }
        }

        #endregion

        private void _reset()
        {
            _resetBoolStates();
           
            _resetTriggers();
            
            Debugging.Log(this, $"[Reset]", Debugging.Type.AnimationMode);
        }

        private void _resetBoolStates()
        {
            if (Mode == EDivaAnimationMode.None)
            {
                _characterAnimator.SetBool(_empty_b, false);
                _frontHairAnimator.SetBool(_empty_b, false);
                _backHairAnimator.SetBool(_empty_b, false);
            }

            _characterAnimator.SetBool(_eatHash_b, false);
            _frontHairAnimator.SetBool(_eatHash_b, false);
            _backHairAnimator.SetBool(_eatHash_b, false);
        }

        private void _resetTriggers()
        {
            _characterAnimator.ResetTrigger(_reactionVoiceHash_t);
            _frontHairAnimator.ResetTrigger(_reactionVoiceHash_t);
            _backHairAnimator.ResetTrigger(_reactionVoiceHash_t);
        }
    }
}