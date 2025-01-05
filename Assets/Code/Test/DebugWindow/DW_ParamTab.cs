using System;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Pools;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class DW_ParamTab : MonoBehaviour,  IPoolEntity
    {
        public event Action OnPressedIncrease;
        public event Action OnPressedDecrease;

        [SerializeField] private Text _textLabel;
        [SerializeField] private Text _textValue;
        [SerializeField] private Button _buttonIncrease;
        [SerializeField] private Button _buttonDecrease;
        
        public void SetLabel(string label)
        {
            _textLabel.text = label;
        }

        public void SetValue(string value)
        {
            _textValue.text = value;
        }

        public void Init(params object[] parameters)
        {
        }

        public void Enable()
        {
            _buttonIncrease.onClick.AddListener(() => OnPressedIncrease?.Invoke());
            _buttonDecrease.onClick.AddListener(() => OnPressedDecrease?.Invoke());
        }

        public void Disable()
        {
            _buttonIncrease.onClick.RemoveAllListeners();
            _buttonDecrease.onClick.RemoveAllListeners();
        }
    }
}