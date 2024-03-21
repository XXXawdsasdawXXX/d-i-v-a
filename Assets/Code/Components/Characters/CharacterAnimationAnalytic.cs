using System;
using Code.Components.Characters.AnimationReader.State;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAnimationAnalytic : CharacterComponent, IGameInitListener, IGameExitListener
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;

        public event Action<CharacterAnimationMode> SwitchModeEvent;


        public void GameInit()
        {
            SubscribeToEvents(true);
        }

        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        public CharacterAnimationMode GetAnimationMode()
        {
            return _characterAnimator.Mode;
        }

        public CharacterAnimationState GetCharacterAnimationState()
        {
            return _animationStateObserver.State;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _characterAnimator.ModeEnteredEvent += CharacterAnimatorOnModeEnteredEvent;
            }
            else
            {
                _characterAnimator.ModeEnteredEvent -= CharacterAnimatorOnModeEnteredEvent;
            }
        }

        private void CharacterAnimatorOnModeEnteredEvent(CharacterAnimationMode mode)
        {
            SwitchModeEvent?.Invoke(mode);
        }
    }
}