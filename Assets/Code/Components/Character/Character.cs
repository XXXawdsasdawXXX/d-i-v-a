﻿using Code.Components.Objects;
using UnityEngine;

namespace Code.Components.Character
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterAnimator _characterAnimator;
        public CharacterAnimator Animator => _characterAnimator;
        
        [SerializeField] private CharacterAudioListener _characterAudioListener;
        public CharacterAudioListener AudioListener => _characterAudioListener;
        

        [SerializeField] private ColliderButton _colliderButton;
        public ColliderButton ColliderButton => _colliderButton;





    }
}