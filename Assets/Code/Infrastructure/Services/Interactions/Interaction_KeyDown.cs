using System;
using Code.Data;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_KeyDown : InteractionObserver, IUpdateListener
    {
        public event Action<EInputWord> OnWorldEntered;
        
        private string _currentInput;

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
                    Log.Info(this, $"[_checkInput] Input matches: {word}.", Log.Type.Input);
                   
                    _currentInput = ""; 
                
                    OnWorldEntered?.Invoke(word);
                 
                    break;
                }
            }
        }
    }
}