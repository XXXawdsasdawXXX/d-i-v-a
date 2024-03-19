using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services;
using UnityEngine;

namespace uWindowCapture
{
    public class TransformDisplayColorGetter : MonoBehaviour, IGameInitListener
    {

        [SerializeField] private UwcWindowTexture _uwcTexture;
        [SerializeField] private Transform _transform;
        [SerializeField] private int _x = 100;
        [SerializeField] private int _y = 100;
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
                var colors = new Color32[_w * _h];
                _texture = new Texture2D(_w, _h, TextureFormat.RGBA32, false);
                GetComponent<Renderer>().material.mainTexture = _texture;
            }
        }


        public Color32 GetColor()
        {
            CreateTextureIfNeeded();

            var window = _uwcTexture.window;
            if (window == null || window.width == 0) return new Color32(255,255,255,255);
            
            Vector3 worldPosition = _transform.position;
            Vector2 screenPosition = _positionService.WorldToScreen(worldPosition);
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Ограничение координат в пределах экрана
            float displayX = Mathf.Clamp(screenPosition.x, 0, screenWidth) + GetTotalWidthOfPreviousDisplays(screenPosition);
            float displayY = Mathf.Clamp(screenPosition.y, 0, screenHeight);
            displayY = screenHeight - displayY; // Переворачиваем ось Y
            
            // Вывод координат на экран
            Debug.Log("mouse X: " + Lib.GetCursorPosition().x + " mouse Y: " + Lib.GetCursorPosition().y);

            // Вывод координат на экран
            _x = Mathf.RoundToInt(displayX);
            _y = Mathf.RoundToInt(displayY);
            Debug.Log("X : " + displayX + ", Y: " + displayY);
            
            _material.color = window.GetPixel(_x, _y);
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