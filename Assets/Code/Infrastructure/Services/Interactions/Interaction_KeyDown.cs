using System;
using Code.Data.Enums;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services.Interactions
{
    public class Interaction_KeyDown : InteractionObserver, IGameTickListener
    {
        private string _currentInput;

        public event Action<InputWord> OnWordEntered;

        
        public void GameTick()
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    // Проверка на символы алфавита и цифры
                    if (kcode >= KeyCode.A && kcode <= KeyCode.Z )
                    {
                        _currentInput += kcode.ToString().ToLower(); // Добавляем символ к строке
                    }
                    else if (kcode == KeyCode.Backspace && _currentInput.Length > 0)
                    {
                        _currentInput = _currentInput.Remove(_currentInput.Length - 1); // Удаление последнего символа
                    }
                    else if( kcode == KeyCode.Space)
                    {
                        _currentInput = "";
                    }

                    // Проверяем строку с перечислением
                    CheckInput();
                }
            }
        }
        
        
        private void CheckInput()
        {
            if (string.IsNullOrEmpty(_currentInput))
            {
                return;
            }
            
            foreach (InputWord word in System.Enum.GetValues(typeof(InputWord)))
            {
                if (_currentInput.Equals(word.ToString()))
                {
                    Debug.Log($"Input matches: {word}");
                    _currentInput = ""; // Сбрасываем строку после успешного совпадения
                    OnWordEntered?.Invoke(word);
                    break;
                }
            }
        }
    }
}