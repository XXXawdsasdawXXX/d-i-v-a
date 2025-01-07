using System;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_KeyDown : InteractionObserver, IGameUpdateListener, IGameInitListener
    {
        private Debugging _debug;
        private string _currentInput;

        public void GameInit()
        {
            _debug = Debugging.Instance;
        }

        public event Action<EInputWord> OnWordEntered;

        public void GameUpdate()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key >= KeyCode.A && key <= KeyCode.Z)
                    {
                        _currentInput += key.ToString().ToLower();
                    }
                    else if (key == KeyCode.Backspace && _currentInput is { Length: > 0 })
                    {
                        _currentInput = _currentInput.Remove(_currentInput.Length - 1); 
                    }
                    else if (key == KeyCode.Space)
                    {
                        _currentInput = "";
                    }

                    _checkInput();
                }
            }
        }

        private void _checkInput()
        {
            if (string.IsNullOrEmpty(_currentInput))
            {
                return;
            }

            foreach (EInputWord word in Enum.GetValues(typeof(EInputWord)))
            {
                if (_currentInput.Equals(word.ToString()))
                {
                    _debug.Log(this, $"Input matches: {word}", Debugging.Type.Input);
                   
                    _currentInput = ""; 
                
                    OnWordEntered?.Invoke(word);
                 
                    break;
                }
            }
        }
    }
}