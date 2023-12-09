using Code.Character.AnimationReader.State;
using UnityEngine;

namespace Code.Character
{
    public class CharacterStateObserver : MonoBehaviour
    {
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
        [SerializeField] private CharacterAnimator _characterAnimator;

    }
}