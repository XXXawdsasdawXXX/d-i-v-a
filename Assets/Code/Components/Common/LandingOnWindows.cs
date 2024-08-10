using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class LandingOnWindows : CommonComponent, IWindowsSpecific, IGameStartListener , IGameExitListener
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColorChecker _colorChecker;

        private bool _isEnable;
        
        public void GameStart()
        {
            SubscribeToEvents(true);
        }
        
        public void GameExit()
        {
            SubscribeToEvents(false);
        }

        #region Events


        private void SubscribeToEvents(bool flag)
        {

            if (flag)
            {
                _colliderButton.OnPressedUp += OnPressedUp;
                _colorChecker.OnFoundedNewColor += OnFoundedNewColor;
            }
            else
            {
                _colliderButton.OnPressedUp -= OnPressedUp;
                _colorChecker.OnFoundedNewColor -= OnFoundedNewColor;
            }
        }

        private void OnPressedUp(Vector2 arg1, float arg2)
        {
            _colorChecker.RefreshLastColor();
        }

        private void OnFoundedNewColor(Color obj)
        {
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
        }
        
        #endregion

        #region Public Methods

        public void SetOffset(Vector2 offset)
        {
            _colorChecker.SetAdditionalOffset(offset);
        }
        
        public void SetEnable(bool isEnable)
        {
            _isEnable = isEnable;
            _colorChecker.SetEnable(isEnable);
            SubscribeToEvents(isEnable);
        }
        
        #endregion
    }
}