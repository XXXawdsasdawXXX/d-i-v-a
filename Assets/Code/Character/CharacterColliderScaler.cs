using System;
using UnityEngine;

namespace Code.Character
{
    public class CharacterColliderScaler : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private CharacterAnimationStateObserver _animationStateObserver;
        [Header("Sizes")] 
        [SerializeField] private SizeParam[] _sizeParams;

        private Vector2 _colliderOffset = new Vector2(0, 0.8f);
        

        [Serializable]
        private struct SizeParam
        {
            public CharacterAnimatorState AnimationState;
            public Vector2 Size;
        }
    }
    
}