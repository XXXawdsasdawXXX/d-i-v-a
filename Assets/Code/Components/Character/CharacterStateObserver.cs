using Code.Components.Character.AnimationReader.State;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterStateObserver : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
        [SerializeField] private CharacterAnimator _characterAnimator;

    }
}