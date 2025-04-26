using System;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Getters;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Entities.Common
{
    public class ColorChecker : CommonComponent, IWindowsSpecific, IInitListener, IUpdateListener
    {
        public event Action<Color> OnFoundedNewColor;

        [Header("Static values")] 
        [SerializeField] private Vector3 _offset;
        private bool _enable;
        private byte _sensitivity;

        [Header("Services")] 
        private DisplayColor _colorAnalyzer;

        [Header("Dynamic value")] 
        [SerializeField] private Color32 _lastColor = Color.white;
        private Vector3 _additionalOffset;

        [Header("Debug")] 
        [SerializeField] private Transform _debugPoint;
        
        public UniTask GameInitialize()
        {
            Container.Instance.GetView<DisplayColorView>().Get(out _colorAnalyzer);
            
            _sensitivity = Container.Instance.FindConfig<SettingsConfig>().ColorCheckSensitivity;

            _trySetDebugPointPosition();
            
            return UniTask.CompletedTask;
        }

        public void GameUpdate()
        {
            if (!_enable)
            {
                return;
            }

            if (_isDifferentColorDetected())
            {
                Color32 newColor = _colorAnalyzer.GetColor(_getCheckPosition());

                string newColorHtml = $"<color=#{ColorUtility.ToHtmlStringRGBA(newColor)}>other</color>";
                string lastColorHtml = $"<color=#{ColorUtility.ToHtmlStringRGBA(_lastColor)}>other</color>";

                _lastColor = newColor;
              
                Log.Info(this, $"Find other color {lastColorHtml} vs {newColorHtml}", Log.Type.Window);
                
                OnFoundedNewColor?.Invoke(_lastColor);
            }
        }

        public void SetEnable(bool enable)
        {
            _enable = enable;
        }

        public void RefreshLastColor()
        {
            _lastColor = _colorAnalyzer.GetColor(_getCheckPosition());
        }

        public void SetAdditionalOffset(Vector3 offset)
        {
            _additionalOffset = offset;
            _trySetDebugPointPosition();
        }

        private bool _isDifferentColorDetected()
        {
            return !_lastColor.Equal(_colorAnalyzer.GetColor(_getCheckPosition()), _sensitivity);
        }

        private Vector3 _getCheckPosition()
        {
            return transform.position + _offset + _additionalOffset;
        }

        private void _trySetDebugPointPosition()
        {
            if (_debugPoint != null)
            {
                _debugPoint.position = _getCheckPosition() + new Vector3(0, _debugPoint.localScale.y / 2 - 0.1f, 0);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(_getCheckPosition(), 0.01f);
        }

        private void OnValidate()
        {
            _trySetDebugPointPosition();
        }
    }
}