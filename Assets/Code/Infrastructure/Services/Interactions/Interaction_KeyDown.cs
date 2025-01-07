using System;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_KeyDown : InteractionObserver, IGameUpdateListener
    {
        private string _currentInput;

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
                    Debug.Log($"Input matches: {word}");
                    _currentInput = ""; 
                    OnWordEntered?.Invoke(word);
                    break;
                }
            }
        }
    }
}