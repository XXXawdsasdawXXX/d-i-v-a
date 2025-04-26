using System;
using System.Linq;
using Code.Data;
using Code.Entities.Common;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Diva
{
    public class DivaModeAdapter : DivaComponent
    {
        [Serializable]
        private class ModeParam
        {
            public EDivaAnimationMode AnimationMode;
            public Vector2 ColliderSize;
            public Vector2 EatPoint;
            public Vector2 HeadPoint;
            public Vector2 LegPoint;
        }

        [Header("Component")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private LandingOnWindows _landingOnWindows;
        [SerializeField] private DivaAnimator _animationModeObserver;

        [Header("Sizes")] 
        [SerializeField] private ModeParam[] _sizeParams;

        private void OnEnable()
        {
            _subscribeToEvents(true);
        }

        private void OnDisable()
        {
            _subscribeToEvents(false);
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

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _animationModeObserver.OnModeEntered += _onModeEnteredEvent;
            }
            else
            {
                _animationModeObserver.OnModeEntered -= _onModeEnteredEvent;
            }
        }

        private void _onModeEnteredEvent(EDivaAnimationMode mode)
        {
            ModeParam modeParam = _sizeParams.FirstOrDefault(p => p.AnimationMode == mode);

            Log.Info(this, $"[_onModeEnteredEvent] Collision switch mode {mode} {modeParam != null}",
                Log.Type.Collision);

            if (modeParam != null)
            {
                _boxCollider2D.size = modeParam.ColliderSize;
                _boxCollider2D.offset = new Vector2(0, modeParam.ColliderSize.y / 2);
                _landingOnWindows.SetOffset(modeParam.LegPoint);
            }
        }
    }
}