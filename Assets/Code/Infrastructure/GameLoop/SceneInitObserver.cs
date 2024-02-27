using System;
using System.Collections;
using Kirurobo;
using UnityEngine;
using UnityEngine.U2D;

namespace Code.Infrastructure.GameLoop
{
    public class SceneInitObserver : MonoBehaviour
    {
        [SerializeField] private PixelPerfectCamera _perfectCamera;
        [SerializeField] private RectTransform _canvas;
        [SerializeField] private float _maxInitSeconds = 3;
        [SerializeField] private UniWindowController _controller;

        [SerializeField] private bool _isInit;
        public event Action InitSceneEvent;
        private void Awake()
        {
            _controller.OnStateChanged += ControllerOnOnStateChanged;
        }

        private void ControllerOnOnStateChanged(UniWindowController.WindowStateEventType type)
        {
            _isInit = true;
            InitSceneEvent?.Invoke();
            Debug.Log(type);
        }

        private IEnumerator AwaitSceneInit()
        {
            var oldSize = _canvas.sizeDelta;
            float elapsedTime = 0f;
    
            while (_canvas.sizeDelta == oldSize && elapsedTime < _maxInitSeconds)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
    
        }
    }
}