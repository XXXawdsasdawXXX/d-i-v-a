using System;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Providers;
using Code.Test;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class LandingOnWindows : CommonComponent, IGameInitListener,IGameStartListener ,IGameTickListener, IGameExitListener
    {
        [Header("static values")]
        [SerializeField] private bool _isUsed;
        [SerializeField] private byte _sensitivity = 3;
        [SerializeField] private Vector3 _landingOffset;
        [Header("components")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        private DisplayColor _colorAnalyzer;
       
        [Header("dynamic value")]
        [SerializeField] private Color32 _lastColor = Color.white;
        private Vector3 _objectOffset;

        [Header("Debug")] 
        [SerializeField] private Transform _debugPoint;
        
        public void GameInit()
        {
            _isUsed = !Extensions.IsMacOs();
         
            if (!_isUsed)
            {
                return;
            }
        
            _colorAnalyzer = Container.Instance.FindGetter<DisplayColorGetter>().Get() as DisplayColor;
        }


        public void GameStart()
        {
            SubscribeToEvents(true);
        }

        public void GameTick()
        {
            if (!_isUsed)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                _sensitivity += 1;
                 Debugging.Instance.Log(this,$"Sentivity = {_sensitivity}", Debugging.Type.Window);
            }
            
            if (IsDifferentColorDetected())
            {
                
                var otherColor = _colorAnalyzer.GetColor(GetCheckPosition());
                var current = $"<color=#{ColorUtility.ToHtmlStringRGBA(_lastColor)}>current</color>";
                var other = $"<color=#{ColorUtility.ToHtmlStringRGBA(otherColor)}>other</color>";
                Debugging.Instance.Log(this,$"Find other color {current} vs {other} " +
                                            $"is dynamic {_rigidbody2D.bodyType == RigidbodyType2D.Dynamic}", Debugging.Type.Window);
                
                switch (_rigidbody2D.bodyType)
                {
                    case RigidbodyType2D.Kinematic:
                        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                        break;
                    case RigidbodyType2D.Dynamic:
                        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                        _rigidbody2D.velocity = Vector2.zero;
                        break;
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

        public void SetOffset(Vector2 offset)
        {
            _objectOffset = offset;
            if (_debugPoint != null)
            {
                _debugPoint.localPosition = _objectOffset + _landingOffset + new Vector3(0,_debugPoint.localScale.y / 2 - 0.4f,0);
            }
        }

        private bool IsDifferentColorDetected()
        {
            return !_lastColor.Equal(_colorAnalyzer.GetColor(GetCheckPosition()), _sensitivity);
        }

        private Vector3 GetCheckPosition()
        {
            return transform.position + _objectOffset + _landingOffset;
        }

        private void ColliderButtonOnUpEvent(Vector2 arg1, float arg2)
        {
            _lastColor = _colorAnalyzer.GetColor(transform.position + _objectOffset + _landingOffset);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position + _objectOffset + _landingOffset, 0.05f);
        }

    }
}