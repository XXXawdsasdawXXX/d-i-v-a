using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    [Serializable]
    public class DW_TimeScale
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Text _textPlaceholder;
        
        public Task Initialize()
        {
            _textPlaceholder.text = Time.timeScale.ToString();
            
            _inputField.onValueChanged.AddListener(value =>
            {
                if (Single.TryParse(value, out float timeScale))
                {
                    _textPlaceholder.text = timeScale.ToString();
                    Time.timeScale = timeScale;
                }
            });
            
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _inputField.onValueChanged.RemoveAllListeners();
        }
    }
}