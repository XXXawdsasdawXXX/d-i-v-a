using System;
using Code.Data.Interfaces;
using UnityEngine;

namespace Code.Infrastructure.Providers
{
    public class CameraProvider : MonoBehaviour, IProvider
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