using System;
using Code.Data.Storages;
using Code.Infrastructure.DI;
using UnityEngine;

namespace Code.Test
{
    public class TestParamPanel : MonoBehaviour
    {
        private CharacterLiveStateStorage _storage;

        private void Start()
        {
            _storage = Container.Instance.FindStorage<CharacterLiveStateStorage>();
        }

        private void OnGUI()
        {
            string text = null;
            foreach (var state in _storage.LiveStates)
            {
                text += $"<color=#86FFBB>{state.Key}</color> {state.Value.Current}\n";
            }
            GUI.Box(new Rect(Screen.width / 2 * -1, 0, 1000, 2000), text);
        }
    }
}