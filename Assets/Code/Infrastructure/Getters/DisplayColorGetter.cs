﻿using Code.Data.Interfaces;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Getters
{
    public class DisplayColorGetter : MonoBehaviour, IGetter
    {
        [SerializeField] private DisplayColor _displayColor;

        private void OnEnable()
        {
            if (Extensions.IsMacOs())
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