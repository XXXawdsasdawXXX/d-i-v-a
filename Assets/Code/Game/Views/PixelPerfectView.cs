using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Game.Views
{
    public class PixelPerfectView : MonoBehaviour, IView, IInitializeListener
    {
        [SerializeField] private PixelPerfectCamera _pixelPerfect;

        public UniTask GameInitialize()
        {
            DisplayInfo display = Screen.mainWindowDisplayInfo;
            
            if (display.height > _pixelPerfect.refResolutionY || display.width > _pixelPerfect.refResolutionX)
            {
                _pixelPerfect.enabled = false;
            }
            
            return UniTask.CompletedTask;
        }

        public object Get()
        {
            return _pixelPerfect;
        }

        public void Get<T>(out T component) where T : class
        {
            if (typeof(T) == typeof(PixelPerfectCamera))
            {
                component = _pixelPerfect as T;
            }
            else
            {
                component = default;
            }
        }
    }
}