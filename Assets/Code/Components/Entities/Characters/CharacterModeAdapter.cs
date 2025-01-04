using System;
using System.Linq;
using Code.Components.Common;
using Code.Data.Enums;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Entities.Characters
{
    public class CharacterModeAdapter : CharacterComponent
    {
        [Header("Component")] [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private LandingOnWindows _landingOnWindows;
        [SerializeField] private CharacterAnimator _animationModeObserver;
        [Header("Sizes")] [SerializeField] private ModeParam[] _sizeParams;

        private void OnEnable()
        {
            SubscribeToEvents(true);
        }

        private void OnDisable()
        {
            SubscribeToEvents(false);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _animationModeObserver.OnModeEntered += OnModeEnteredEvent;
            }
            else
            {
                _animationModeObserver.OnModeEntered -= OnModeEnteredEvent;
            }
        }

        public Vector3 GetWorldEatPoint()
        {
            ModeParam modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == _animationModeObserver.Mode);
            if (modeParam != null)
            {
                Vector2 localPosition = modeParam.EatPoint;
                return transform.TransformPoint(localPosition);
            }

            return transform.position;
        }

        public Vector3 GetWorldHeatPoint()
        {
            ModeParam modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == _animationModeObserver.Mode);
            if (modeParam != null)
            {
                Vector2 localPosition = modeParam.HeadPoint;
                return transform.TransformPoint(localPosition);
            }

            return transform.position;
        }

        public Vector3 GetLegPoint()
        {
            ModeParam modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == _animationModeObserver.Mode);
            if (modeParam != null)
            {
                Vector2 localPosition = modeParam.LegPoint;
                return transform.TransformPoint(localPosition);
            }

            return transform.position;
        }

        private void OnModeEnteredEvent(CharacterAnimationMode mode)
        {
            ModeParam modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == mode);
            Debugging.Instance.Log($"Collision switch mode {mode} {modeParam != null}", Debugging.Type.Collision);
            if (modeParam != null)
            {
                _boxCollider2D.size = modeParam.ColliderSize;
                _boxCollider2D.offset = new Vector2(0, modeParam.ColliderSize.y / 2);
                _landingOnWindows.SetOffset(modeParam.LegPoint);
            }
        }


        [Serializable]
        private class ModeParam
        {
            public CharacterAnimationMode AnimationMode;
            public Vector2 ColliderSize;
            public Vector2 EatPoint;
            public Vector2 HeadPoint;
            public Vector2 LegPoint;
        }
    }
}