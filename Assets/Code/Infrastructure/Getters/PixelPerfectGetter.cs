using Code.Data;
using Code.Infrastructure.GameLoop;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Infrastructure.Getters
{
    public class PixelPerfectGetter : MonoBehaviour, IGetter, IGameInitListener
    {
        [SerializeField] private PixelPerfectCamera _pixelPerfect;

        public void GameInit()
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