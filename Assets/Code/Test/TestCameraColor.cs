using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Providers;
using UnityEngine;

namespace Code.Test
{
    public class TestCameraColor : MonoBehaviour
    {
        [SerializeField] private Color32 _cameraColor;
        private Camera _camera;
        
#if UNITY_EDITOR
        public void Start()
        {
            _camera = Container.Instance.FindGetter<CameraGetter>().Get() as Camera;
            if (_camera == null)
            {
                _camera = FindObjectOfType<Camera>();
            }
            _camera.backgroundColor = _cameraColor;
        }
#endif
    }
}