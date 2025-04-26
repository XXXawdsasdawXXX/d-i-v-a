using Code.Data;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Infrastructure.Services.Getters
{
    public class CameraView : MonoBehaviour, IView
    {
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public PixelPerfectCamera PixelPerfectCamera { get; private set; }

        public object Get()
        {
            return Camera;
        }

        private void OnValidate()
        {
            if (Camera == null)
            {
                TryGetComponent(out Camera cameraComponent);
                Camera = cameraComponent;
            }
        }

        public Vector3 WorldToScreen(Vector2 screenPoint)
        {
            Vector3 worldPoint = PixelPerfectCamera != null && PixelPerfectCamera.enabled
                ? PixelPerfectCamera.RoundToPixel(Camera.ScreenToWorldPoint(screenPoint))
                : Camera.ScreenToWorldPoint(screenPoint);
            return new Vector3(worldPoint.x, worldPoint.y, 0);
        }
    }
}