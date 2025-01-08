using Code.Data;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Entities.Common
{
    public class LandingOnWindows : CommonComponent, IWindowsSpecific, IGameStartListener, IGameExitListener
    {
        [Header("Components")] 
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColorChecker _colorChecker;

        [Header("Dynamic Data")] 
        [SerializeField] private bool _isEnable = true;

        public void GameStart()
        {
            _setEnable(_isEnable);
        }

        public void GameExit()
        {
            _subscribeToEvents(false);
        }
        
        public void SetOffset(Vector2 offset)
        {
            _colorChecker.SetAdditionalOffset(offset);
        }
        
        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _colliderButton.OnPressedUp += _onPressedUp;
                _colorChecker.OnFoundedNewColor += _onFoundedNewColor;
            }
            else
            {
                _colliderButton.OnPressedUp -= _onPressedUp;
                _colorChecker.OnFoundedNewColor -= _onFoundedNewColor;
            }
        }

        private void _onPressedUp(Vector2 arg1, float arg2)
        {
#if DEBUGGING
            Debugging.Log(this, $"{gameObject.name} [OnPressedUp]", Debugging.Type.Window);
#endif
            _colorChecker.RefreshLastColor();
        }

        private void _onFoundedNewColor(Color obj)
        {
#if DEBUGGING
            Debugging.Log(this, $"{gameObject.name} [OnFoundedNewColor]", Debugging.Type.Window);
#endif
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

        private void _setEnable(bool isEnable)
        {
#if DEBUGGING
            Debugging.Log(this, $"{gameObject.name} [SetEnable] {isEnable}", Debugging.Type.Window);
#endif

            _isEnable = isEnable;
            
            _colorChecker.SetEnable(isEnable);
            
            _subscribeToEvents(isEnable);
        }
    }
}