﻿using Code.Components.Character.AnimationReader.State;
using Code.Components.Objects;
using UnityEngine;

namespace Code.Components.Character
{
    public class Character : MonoBehaviour
    {
        
        [SerializeField] private CharacterAudioListener _characterAudioListener;
        public CharacterAudioListener AudioListener => _characterAudioListener;
        

        [SerializeField] private ColliderButton _colliderButton;
        public ColliderButton ColliderButton => _colliderButton;

        
        [Header("Animation")]
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;
        
        
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
        public CharacterAnimationStateObserver AnimationStateObserver => _animationStateObserver;
        
    }
}