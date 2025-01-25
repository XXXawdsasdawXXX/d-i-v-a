using Code.Data;
using Code.Infrastructure.GameLoop;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Infrastructure.Services.Getters
{
    public class PixelPerfectGetter : MonoBehaviour, IGetter, IInitListener
    {
        [SerializeField] private PixelPerfectCamera _pixelPerfect;

        public void GameInitialize()
        {
            DisplayInfo display = Screen.mainWindowDisplayInfo;
            if (display.height > _pixelPerfect.refResolutionY || display.width > _pixelPerfect.refResolutionX)
            {
                _pixelPerfect.enabled = false;
            }
        }

        public object Get()
        {
            return _pixelPerfect;
        }
    }
}