using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Providers;
using UnityEngine;

namespace Code.Test
{
    public class TestCameraColor : MonoBehaviour, IGameInitListener
    {
        [SerializeField] private Color32 _cameraColor;
        private Camera _camera;
        
#if UNITY_EDITOR
        public void GameInit()
        {
            var camera = Container.Instance.FindProvider<CameraProvider>().Get() as Camera;
            _camera.backgroundColor = _cameraColor;
        }
#endif
    }
}