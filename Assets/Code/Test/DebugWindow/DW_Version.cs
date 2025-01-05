using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    [Serializable]
    public class DW_Version
    {
        [SerializeField] private Text _textVersion;
        
        public Task Initialize()
        {
            _textVersion.text = Application.version;
            
            return Task.CompletedTask;
        }
    }
}