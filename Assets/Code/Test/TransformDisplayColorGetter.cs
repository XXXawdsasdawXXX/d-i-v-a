﻿using Code.Infrastructure.DI;
using Code.Services;
using UnityEngine;

namespace uWindowCapture
{
    public class TransformDisplayColorGetter : MonoBehaviour
    {
        public Camera mainCamera; // Ссылка на камеру, которую вы используете в вашем проекте

        [SerializeField] UwcWindowTexture uwcTexture;
        [SerializeField] private Transform _transform;

        Material material_;
        private PositionService _positionService;

        void Start()
        {
            material_ = GetComponent<Renderer>().material;
//            _positionService = Container.Instance.FindService<PositionService>();
        }

        void Update()
        {
            Example();

        }

    

        [SerializeField] int x = 100;
        [SerializeField] int y = 100;
        [SerializeField] int w = 1;
        [SerializeField] int h = 1;
    
        public Texture2D texture;
        Color32[] colors;

        void CreateTextureIfNeeded()
        {
            if (!texture || texture.width != w || texture.height != h)
            {
                colors = new Color32[w * h];
                texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
                GetComponent<Renderer>().material.mainTexture = texture;
            }
        }

     
        void Example()
        {
            CreateTextureIfNeeded();

            var window = uwcTexture.window;
            if (window == null || window.width == 0) return;
            
            Vector3 worldPosition = _transform.position;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            

            // Получение ширины и высоты экрана
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Ограничение координат в пределах экрана
            float displayX = Mathf.Clamp(screenPosition.x, 0, screenWidth);
            float displayY = Mathf.Clamp(screenPosition.y, 0, screenHeight);

            // Вывод координат на экран
            Debug.Log("X Coordinate on Display: " + displayX + ", Y Coordinate on Display: " + displayY);

            // Вывод координат на экран
            x = Mathf.RoundToInt(displayX);
            y = Mathf.RoundToInt(displayY);
            Debug.Log("X : " + displayX + ", Y: " + displayY);

            // GetPixels() can be run in another thread
            
            if (window.GetPixels(colors, x, y, w, h)) {
                texture.SetPixels32(colors);
                texture.Apply();
            }
        }
    }
}