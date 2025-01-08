using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Entities.Common
{
    public class MaterialAdapter : CommonComponent
    {
        [SerializeField] protected Material _material;
        [SerializeField] private StateType stateType;
        [SerializeField] private bool _isOn;
        [Space] [SerializeField] private FloatValueType valueType;
        [SerializeField] private float _newFloatValue = 3.14f; // Новое значение для угла в радианах

        private Dictionary<StateType, bool> States;
        private Dictionary<FloatValueType, bool> Floats;

        public enum StateType
        {
            SHINE_ON,
            HOLOGRAM_ON,
            GLITCH_ON,
            GHOST_ON,
            CHANGECOLOR_ON,
            ONLYOUTLINE_ON,
            FADE_ON,
            OUTBASE_ON,
            GRADIENT_ON,
            DOODLE_ON
        }

        public enum FloatValueType
        {
            _ShineRotate,
            _ColorChangeTolerance, //Range(0, 1)) = 0.25 //123,
            _ColorChangeLuminosity,
            _ColorChangeTolerance2,
            _TextureScrollXSpeed,
            _TextureScrollYSpeed
        }

        public enum ColorValueType
        {
            _ColorChangeTarget, // "Color to change" 
            _ColorChangeNewCol,
            _ColorChangeTarget2,
            _ColorChangeNewCol2,
        }

        #region Base methods

        public bool GetStateValue(StateType stateType) => _material != null && _material.shader.isSupported &&
                                                          _material.IsKeywordEnabled(stateType.ToString());

        protected void SetFloatValue(FloatValueType floatValueType, float value)
        {
            if (_material != null && _material.HasProperty(floatValueType.ToString()))
            {
                _material.SetFloat(floatValueType.ToString(), value);
            }
            else
            {
                Debug.LogError("Материал не содержит свойство с именем " + floatValueType);
            }
        }

        protected void SetState(StateType stateType, bool isActive)
        {
            if (_material != null && _material.shader.isSupported)
            {
                if (isActive && !_material.IsKeywordEnabled(stateType.ToString()))
                {
                    _material.EnableKeyword(stateType.ToString());
                }
                else
                {
                    _material.DisableKeyword(stateType.ToString());
                }
            }
            else
            {
                Debug.LogError("Материал не поддерживает директиву " + stateType);
            }
        }

        #endregion

        #region Editor

        public void Editor_RefreshState()
        {
            SetState(stateType, _isOn);
        }

        public void Editor_RefreshValue()
        {
            SetFloatValue(valueType, _newFloatValue);
        }

        #endregion

        protected void Clear()
        {
            foreach (object value in Enum.GetValues(typeof(StateType)))
            {
                Console.WriteLine(value);
                SetState((StateType)value, false);
            }
        }
    }
}