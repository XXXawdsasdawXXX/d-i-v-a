using System;
using UnityEngine;

namespace Code.Test
{
    public class TestCameraColor : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Color32 _cameraColor;
#if UNITY_EDITOR
        private void Start()
        {
            _camera.backgroundColor = _cameraColor;
            
        }
        
#endif
    }
}