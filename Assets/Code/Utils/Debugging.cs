using System;
using System.Linq;
using System.Text;
using UnityEditor;
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
            DiContainer,
            Time,
            BehaviorTree,
            LiveState,
            SaveLoad,
            ButtonSprite,
            Position,
            GameState,
            Grass,
            CustomAction,
            Interaction,
            Hand,
            Items,
            Window,
            CharacterCondition,
            VFX,
            Input
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
            DebugParam debugParam = _debugParams.FirstOrDefault(d => d.Type == type);
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

        public void Log(object invoker, string message, Type type = Type.None)
        {
            DebugParam debugParam = _debugParams.FirstOrDefault(d => d.Type == type);
            if (debugParam != null)
            {
                if (debugParam.Active)
                {
                    ColorLog(
                        $"{InsertSpaceBeforeUppercase(type.ToString()).ToUpper()} {invoker.GetType().Name}: {message}",
                        debugParam.Color);
                }
            }
            else
            {
                ColorLog(message, Color.white);
            }
        }

        public static void LogError(object obj, string message)
        {
            ColorLog($"{obj.GetType()} {message}", Color.red);
        }

        public static void LogError(string message)
        {
            ColorLog(message, Color.red);
        }

        private static void ColorLog(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }

        private static string InsertSpaceBeforeUppercase(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (char character in input)
            {
                if (char.IsUpper(character))
                {
                    result.Append(' ');
                }

                result.Append(character);
            }

            return result.ToString().Trim();
        }

#if UNITY_EDITOR

        public void DisableAll()
        {
            foreach (DebugParam debugParam in _debugParams)
            {
                debugParam.Active = false;
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}