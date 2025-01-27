using Code.Data;
using UnityEngine;

namespace Code.Infrastructure.Services.Getters
{
    public class CameraGetter : MonoBehaviour, IGetter
    {
        [SerializeField] private Camera _camera;
        
        public object Get()
        {
            return _camera;
        }

        private void OnValidate()
        {
            if (_camera == null)
            {
                TryGetComponent(out _camera);
            }
        }

        public void Get<T>(out T component) where T : class
        {
            if (typeof(T) == typeof(Camera))
            {
                component = _camera as T;
            }
            else
            {
                component = default;
            }
        }
    }
}