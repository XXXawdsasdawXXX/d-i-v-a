﻿using System;
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
            string text = $"{Math.Round(_timer.GetCurrentTick())}/{Math.Round(_timer.GetTickDuration())}";
            
            _textTick.text = text;
        }
        
    }
}