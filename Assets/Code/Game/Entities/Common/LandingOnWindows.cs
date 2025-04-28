using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Entities.Common
{
    public class LandingOnWindows : CommonComponent, IWindowsSpecific, IStartListener, ISubscriber
    {
        [Header("Components")] 
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private ColliderButton _colliderButton;
        [SerializeField] private ColorChecker _colorChecker;

        public void Subscribe()
        {
            _colliderButton.OnPressedUp += _onPressedUp;
            _colorChecker.OnFoundedNewColor += _onFoundedNewColor;
        }

        public UniTask GameStart()
        {
            _colorChecker.SetEnable(true);

            return UniTask.CompletedTask;
        }

        public void Unsubscribe()
        {
            _colliderButton.OnPressedUp -= _onPressedUp;
            _colorChecker.OnFoundedNewColor -= _onFoundedNewColor;
        }

        public void SetOffset(Vector2 offset)
        {
            _colorChecker.SetAdditionalOffset(offset);
        }

        private void _onPressedUp(Vector2 arg1, float arg2)
        {
            Log.Info(this, $"{gameObject.name} [OnPressedUp]", Log.Type.Window);

            _colorChecker.RefreshLastColor();
        }

        private void _onFoundedNewColor(Color obj)
        {
            Log.Info(this, $"{gameObject.name} [OnFoundedNewColor]", Log.Type.Window);

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
    }
}