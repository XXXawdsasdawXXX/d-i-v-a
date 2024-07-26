﻿using Code.Data.Interfaces;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;
using uWindowCapture;

namespace Code.Providers
{
    public class DisplayColorProvider: MonoBehaviour, IProvider, IGameInitListener
    {
        [SerializeField] private DisplayColor _displayColor;

        public void GameInit()
        {
            var isUsed = !Extensions.IsMacOs();
            if (!isUsed)
            {
               var components = GetComponentsInChildren<MonoBehaviour>();
               foreach (var behaviour in components)
               {
                   behaviour.enabled = false;
               }
            }    
        }

        public object Get()
        {
            return _displayColor;
        }
    }
}