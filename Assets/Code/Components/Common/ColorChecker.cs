using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using uWindowCapture;

namespace Code.Components.Objects
{
    public class ColorChecker : CommonComponent, IGameInitListener, IGameTickListener, IGameExitListener
    {
        [SerializeField] private bool _isCheck;
        [SerializeField] private Color32 _lastColor = Color.red;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private TransformDisplayColorGetter _colorAnalyzer;
        [SerializeField] private ColliderButton _colliderButton;

        public void GameInit()
        {
            SubscribeToEvents(true);
        }

        private void ColliderButtonOnUpEvent(Vector2 arg1, float arg2)
        {
            _lastColor = _colorAnalyzer.GetColor();
        }


        public void GameTick()
        {
            if (!_isCheck)
            {
                return;
            }
            
            if (!_lastColor.Equal(_colorAnalyzer.GetColor(), 3) && _rigidbody2D.bodyType != RigidbodyType2D.Kinematic)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody2D.velocity = Vector2.zero;
                _lastColor = _colorAnalyzer.GetColor();
            }
        }

        public void GameExit()
        {
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
    }
}