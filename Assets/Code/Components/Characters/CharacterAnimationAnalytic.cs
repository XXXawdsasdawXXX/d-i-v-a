using Code.Components.Characters.AnimationReader.State;
using Code.Data.Enums;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAnimationAnalytic : CharacterComponent
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
     
        
        public CharacterAnimationMode GetAnimationMode()
        {
            return _characterAnimator.Mode;
        }
        
        public CharacterAnimationState GetCharacterAnimationState()
        {
            return _animationStateObserver.State;
        }
    }
}