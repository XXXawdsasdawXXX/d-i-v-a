using System;
using Code.Data;
using UnityEngine;

namespace Code.Infrastructure.Services.Getters
{
    public class CameraGetter : MonoBehaviour, IGetter
    {
        [SerializeField] private Camera _camera;

        public Type Type => typeof(Camera);

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
    }
}