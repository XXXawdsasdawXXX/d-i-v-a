using System;
using System.Linq;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Characters
{
    public class CharacterModeAdapter : CharacterComponent
    {
        [Header("Component")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private CharacterAnimator _animationModeObserver;
        [Header("Sizes")] 
        [SerializeField] private ModeParam[] _sizeParams;

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }

        public Vector3 GetWorldEatPoint()
        {
            var modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == _animationModeObserver.Mode);
            if (modeParam != null)
            {
                var localPosition = modeParam.EatPoint;
                return transform.TransformPoint(localPosition);
            }

            return transform.position;
        }

        public Vector3 GetWorldHeatPoint()
        {
            var modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == _animationModeObserver.Mode);
            if (modeParam != null)
            {
                var localPosition = modeParam.HeadPoint;
                return transform.TransformPoint(localPosition);
            }

            return transform.position;
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
            var modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == mode);
            Debugging.Instance.Log($"Collision switch mode {mode} {modeParam != null}", Debugging.Type.Collision);
            if (modeParam != null)
            {
                _boxCollider2D.size = modeParam.ColliderSize;
                _boxCollider2D.offset = new Vector2(0, modeParam.ColliderSize.y / 2);
            }
        }

        [Serializable]
        private class ModeParam
        {
            public CharacterAnimationMode AnimationMode;
            public Vector2 ColliderSize;
            public Vector2 EatPoint;
            public Vector2 HeadPoint;
        }
    }
    
}