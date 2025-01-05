using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    [Serializable]
    public class DW_Tick 
    {
        [SerializeField] private Text _textTick;
        
        private TimeObserver _timer;
        
        public Task Initialize()
        {
            _timer = Container.Instance.FindService<TimeObserver>();
            
            return Task.CompletedTask;
        }

        public void Refresh()
        {
            KeyValuePair<float, float> time = _timer.GetTimeState();
            
            string text = $"{Math.Round(time.Key)}/{Math.Round(time.Value)}={Math.Round(time.Key / time.Value, 1)}";
            
            _textTick.text = text;
        }
        
    }
}