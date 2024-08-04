using System;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Providers;
using Code.Test;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class LandingOnWindows : CommonComponent, IGameInitListener, IGameTickListener, IGameExitListener
    {
        [Header("static values")]
        [SerializeField] private bool _isUsed;
        [SerializeField] private byte _sensitivity = 3;
        [SerializeField] private Vector2 _offset;
        [Header("components")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        private DisplayColor _colorAnalyzer;
        [Header("dynamic value")]
        [SerializeField] private Color32 _lastColor = Color.white;

        public void GameInit()
        {
            _isUsed = !Extensions.IsMacOs();
            if (!_isUsed)
            {
                return;
            }
        
            _colorAnalyzer = Container.Instance.FindGetter<DisplayColorGetter>().Get() as DisplayColor;
            SubscribeToEvents(true);
        }


        public void GameTick()
        {
            if (!_isUsed)
            {
                return;
            }
            
            if (IsDifferentColorDetected())
            {
                var otherColor = _colorAnalyzer.GetColor(transform.position + _offset.AsVector3());
                var current = $"<color=#{ColorUtility.ToHtmlStringRGBA(_lastColor)}>current</color>";
                var other = $"<color=#{ColorUtility.ToHtmlStringRGBA(otherColor)}>other</color>";
                Debugging.Instance.Log(this,$"Find other color {current} vs {other} " +
                                            $"is dynamic {_rigidbody2D.bodyType == RigidbodyType2D.Dynamic}", Debugging.Type.Window);
                
                if (_rigidbody2D.bodyType == RigidbodyType2D.Dynamic)
                {
                    _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                    _rigidbody2D.velocity = Vector2.zero;
                }
                else if(_rigidbody2D.bodyType == RigidbodyType2D.Kinematic)
                {
                    _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                }
                _lastColor = otherColor;
            }
        }

        public void GameExit()
        {
            if (!_isUsed)
            {
                return;
            }
            SubscribeToEvents(false);
        }

        public void SetOffset(Vector2 offset)
        {
            _offset = offset;
        }

        private bool IsDifferentColorDetected()
        {
            return !_lastColor.Equal(_colorAnalyzer.GetColor(transform.position + _offset.AsVector3()), _sensitivity);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.UpEvent += ColliderButtonOnUpEvent;
            }
            else
            {
                _colliderButton.UpEvent -= ColliderButtonOnUpEvent;
            }
        }

        private void ColliderButtonOnUpEvent(Vector2 arg1, float arg2)
        {
            _lastColor = _colorAnalyzer.GetColor(transform.position + _offset.AsVector3());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + _offset.AsVector3(), 0.05f);
        }
    }
}