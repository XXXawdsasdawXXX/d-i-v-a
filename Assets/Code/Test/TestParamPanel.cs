using System.Collections.Generic;
using Code.Data.Enums;
using Code.Data.Storages;
using Code.Data.Value;
using Code.Infrastructure.DI;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Test
{
    public class TestParamPanel : MonoBehaviour
    {
        [SerializeField] private Text _paramText;
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _console;

        private LiveStateStorage _storage;

        private void Start()
        {
            _storage = Container.Instance.FindStorage<LiveStateStorage>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _body.SetActive(!_body.activeSelf);
                _console.SetActive(!_console.activeSelf);
            }

            if (!_body.activeSelf)
            {
                return;
            }

            string text = null;
            foreach (KeyValuePair<LiveStateKey, CharacterLiveState> state in _storage.LiveStates)
            {
                text +=
                    $"<color=#86FFBB>{state.Key}</color>: {state.Value.Current,11}/{state.Value.Max}\t{state.Value.GetPercent(),15}%\n";
            }

            _paramText.text = text;
        }
    }
}