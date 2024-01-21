using System;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class TestParamPanel : MonoBehaviour
    {
        private CharacterLiveStateStorage _storage;
        [SerializeField] private Text _paramText;
        

        private void Start()
        {
            _storage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
        }

        private void Update()
        {
            string text = null;
            foreach (var state in _storage.LiveStates)
            {
                text += $"<color=#86FFBB>{state.Key}</color>\t{state.Value.Current}\t{state.Value.GetPercent()}\n"; 
            }
            _paramText.text = text;
        }

    }
}