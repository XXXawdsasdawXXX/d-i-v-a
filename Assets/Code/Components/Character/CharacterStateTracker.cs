using Code.Components.Character.AnimationReader.Mode;
using Code.Components.Character.AnimationReader.State;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterStateTracker : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimationState GetCurrentAnimationState() => _animationStateObserver.State;
        public CharacterAnimationMode GetCurrentAnimationMode() => _characterAnimator.Mode;
        
        
        
    }
}