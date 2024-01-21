using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Code.Utils
{
    public class Debugging : MonoBehaviour
    {
        public static Debugging Instance;

        public enum Type
        {
            None,
            AnimationState,
            AnimationMode,
            Collision,
            Micro,
            DiContainer,
            Time,
            BehaviorTree,
            LiveState,
            SaveLoad,
            ButtonSprite,
            Item,
            Position
        }

        [Serializable]
        private class DebugParam
        {
            public Type Type;
            public bool Active = true;
            public Color Color = Color.white;
        }

        [SerializeField] private DebugParam[] _debugParams;

        private void Awake()
        {
            Instance = this;
        }

        public void Log(string message, Type type = Type.None)
        {
            var debugParam = _debugParams.FirstOrDefault(d => d.Type == type);
            if (debugParam != null)
            {
                if (debugParam.Active)
                {
                    ColorLog($"{InsertSpaceBeforeUppercase(type.ToString()).ToUpper()}: {message}", debugParam.Color);
                }
            }
            else
            {
                ColorLog(message, Color.white);
            }
        }

        public void TestLog(string message)
        {
            ColorLog(message, Color.green);
        }

        public void ErrorLog(string message)
        {
            ColorLog(message, Color.red);
        }

        private void ColorLog(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }
        
        private  string InsertSpaceBeforeUppercase(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char character in input)
            {
                if (char.IsUpper(character))
                {
                    // Вставляем пробел перед заглавной буквой
                    result.Append(' ');
                }

                // Добавляем текущий символ в результат
                result.Append(character);
            }

            return result.ToString().Trim(); // Удаляем возможный пробел в начале строки
        }
    }
}