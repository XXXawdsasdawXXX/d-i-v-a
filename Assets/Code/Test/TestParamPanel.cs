﻿using Code.Data.Storages;
using Code.Infrastructure.DI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class TestParamPanel : MonoBehaviour
    {
        private LiveStateStorage _storage;
        [SerializeField] private Text _paramText;
        

        private void Start()
        {
            _storage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        private void Update()
        {
            string text = null;
            foreach (var state in _storage.LiveStates)
            {
                text += $"<color=#86FFBB>{state.Key}</color>: {state.Value.Current,11}/{state.Value.Max}\t{state.Value.GetPercent(),15}%\n"; 
            }
            _paramText.text = text;
        }

    }
}