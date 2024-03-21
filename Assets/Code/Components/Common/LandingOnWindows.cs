using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using uWindowCapture;

namespace Code.Components.Objects
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
        [SerializeField] private TransformDisplayColorGetter _colorAnalyzer;
        [Header("dynamic value")]
        [SerializeField] private Color32 _lastColor = Color.white;

        public void GameInit()
        {
            _isUsed = !Extensions.IsMacOs();
            if (!_isUsed)
            {
                return;
            }
            SubscribeToEvents(true);
        }


        public void GameTick()
        {
            if (!_isUsed)
            {
                return;
            }
            
            if (IsDifferentColorDetected() && _rigidbody2D.bodyType != RigidbodyType2D.Kinematic)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody2D.velocity = Vector2.zero;
                _lastColor = _colorAnalyzer.GetColor(transform.position);
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
    }
}