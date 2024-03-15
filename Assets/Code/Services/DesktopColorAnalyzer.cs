using System;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Kirurobo;
using UnityEngine;

namespace Code.Services
{
    public class DesktopColorAnalyzer: IService, IGameInitListener
    {
        private UniWindowController _uniWindow;
        
        private const float TOLERANCE = 0.25f;

        public void GameInit()
        {
            _uniWindow = Container.Instance.GetUniWindowController();
        }

        public Color GetColor(Vector2 screenPosition)
        {
            return _uniWindow.TryGetColor(screenPosition, out var color) ? color : Color.black;
        }

        public bool IsCurrentColor(Vector2 screenPosition ,Color color, out Color newColor)
        {
             newColor = GetColor(screenPosition);
            return CompareColors(color, newColor);
        }

        public  bool CompareColors(Color color1, Color color2)
        {
            // Calculate the difference in each color channel.
            float rDiff = Mathf.Abs(color1.r - color2.r);
            float gDiff = Mathf.Abs(color1.g - color2.g);
            float bDiff = Mathf.Abs(color1.b - color2.b);

            // Calculate the total color difference.
            float totalDiff = rDiff + gDiff + bDiff;

            // Return true if the total difference is less than the tolerance.
            return totalDiff <= TOLERANCE;
        }
    }
}