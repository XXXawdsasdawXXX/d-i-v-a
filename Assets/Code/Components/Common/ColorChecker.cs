using System;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Getters;
using Code.Test;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class ColorChecker : CommonComponent, IWindowsSpecific,IGameInitListener, IGameTickListener
    {
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
        
        public event Action<Color> OnFoundedNewColor; 

        public void GameInit()
        {
            _colorAnalyzer = Container.Instance.FindGetter<DisplayColorGetter>().Get() as DisplayColor;
            _sensitivity = Container.Instance.FindConfig<SettingsConfig>().ColorCheckSensitivity;
        
            TrySetDebugPointPosition();
        }
        
        public void GameTick()
        {
            if (!_enable)
            {
                return;
            }
            
            if (IsDifferentColorDetected())
            {
                var newColor = _colorAnalyzer.GetColor(GetCheckPosition());
                
                var newColorHtml = $"<color=#{ColorUtility.ToHtmlStringRGBA(newColor)}>other</color>";
                var lastColorHtml = $"<color=#{ColorUtility.ToHtmlStringRGBA(_lastColor)}>other</color>";
                Debugging.Instance.Log(this,$"Find other color {lastColorHtml} vs {newColorHtml}", Debugging.Type.Window);
               
                _lastColor = newColor;
             
                OnFoundedNewColor?.Invoke(_lastColor);
            }
        }

        public void SetEnable(bool enable)
        {
            _enable = enable;
        }

        public void RefreshLastColor()
        {
            _lastColor = _colorAnalyzer.GetColor(GetCheckPosition());;
        }

        public void SetAdditionalOffset(Vector3 offset)
        {
            _additionalOffset = offset;
            TrySetDebugPointPosition();
        }

        private bool IsDifferentColorDetected()
        {
            return !_lastColor.Equal(_colorAnalyzer.GetColor(GetCheckPosition()), _sensitivity);
        }

        private Vector3 GetCheckPosition()
        {
            return transform.position + _offset + _additionalOffset;
        }

        private void TrySetDebugPointPosition()
        {
            if (_debugPoint != null)
            {
                _debugPoint.position = GetCheckPosition() + new Vector3(0,_debugPoint.localScale.y / 2 - 0.1f,0);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(GetCheckPosition(), 0.01f);
        }

        private void OnValidate()
        {
            TrySetDebugPointPosition();
        }
    }
}