using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;
using uWindowCapture;

namespace Code.Infrastructure.Services
{
    public class DisplayColor : MonoBehaviour, IWindowsSpecific, IGameInitListener
    {
        [SerializeField] private UwcWindowTexture _uwcTexture;
        [SerializeField] private int _w = 1;
        [SerializeField] private int _h = 1;

        private Texture2D _texture;
        private Material _material;

        private PositionService _positionService;

        public void GameInit()
        {
            _material = GetComponent<Renderer>().material;
            _positionService = Container.Instance.FindService<PositionService>();
        }

        private void CreateTextureIfNeeded()
        {
            if (!_texture || _texture.width != _w || _texture.height != _h)
            {
                Color32[] colors = new Color32[_w * _h];
                _texture = new Texture2D(_w, _h, TextureFormat.RGBA32, false);
                GetComponent<Renderer>().material.mainTexture = _texture;
            }
        }


        public Color32 GetColor(Vector3 worldPosition)
        {
            CreateTextureIfNeeded();

            UwcWindow window = _uwcTexture.window;
            if (window == null || window.width == 0) return new Color32(255, 255, 255, 255);

            Vector2 screenPosition = _positionService.WorldToScreen(worldPosition);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;


            float displayX = Mathf.Clamp(screenPosition.x, 0, screenWidth) +
                             GetTotalWidthOfPreviousDisplays(screenPosition);
            float displayY = Mathf.Clamp(screenPosition.y, 0, screenHeight);
            displayY = screenHeight - displayY; // Переворачиваем ось Y

            int x = Mathf.RoundToInt(displayX);
            int y = Mathf.RoundToInt(displayY);

            _material.color = window.GetPixel(x, y);
            return _material.color;
        }

        private static float GetTotalWidthOfPreviousDisplays(Vector2 screenPosition)
        {
            float totalWidthOfPreviousDisplays = 0f;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                if (screenPosition.x >= Display.displays[i].systemWidth)
                {
                    totalWidthOfPreviousDisplays += Display.displays[i].renderingWidth;
                }
                else
                {
                    break;
                }
            }

            return totalWidthOfPreviousDisplays;
        }
    }
}