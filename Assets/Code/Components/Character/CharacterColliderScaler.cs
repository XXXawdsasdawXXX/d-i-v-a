using System;
using System.Linq;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Character
{
    public class CharacterColliderScaler : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private CharacterAnimator _animationModeObserver;
        [Header("Sizes")] 
        [SerializeField] private SizeParam[] _sizeParams;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
             _animationModeObserver.ModeEnteredEvent += OnModeEnteredEvent;   
        }

        private void UnsubscribeToEvents()
        {
            _animationModeObserver.ModeEnteredEvent -= OnModeEnteredEvent;
        }

        private void OnModeEnteredEvent(CharacterAnimationMode mode)
        {
            var sizeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == mode);
            Debugging.Instance.Log($"Collision switch mode {mode} {sizeParam != null}", Debugging.Type.Collision);
            if (sizeParam != null)
            {
                _boxCollider2D.size = sizeParam.Size;
                _boxCollider2D.offset = new Vector2(0, sizeParam.Size.y / 2);
            }
        }

        [Serializable]
        private class SizeParam
        {
            public CharacterAnimationMode AnimationMode;
            public Vector2 Size;
        }
    }
    
}