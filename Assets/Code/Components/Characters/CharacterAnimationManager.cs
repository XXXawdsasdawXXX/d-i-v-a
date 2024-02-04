using Code.Components.Character.LiveState;
using Code.Components.Characters.AnimationReader.State;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterAnimationManager : MonoBehaviour
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