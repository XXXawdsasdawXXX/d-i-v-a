using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Code.Utils
{
    public class Debugging : MonoBehaviour
    {
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

        private static DebugParam[] _params;
        private void Awake()
        {
            _params = _debugParams;
        }

        public static void Log(string message, Type type = Type.None)
        {
            DebugParam debugParam = _params.FirstOrDefault(d => d.Type == type);
      
            if (debugParam != null)
            {
                if (debugParam.Active)
                {
                    _colorLog($"{_insertSpaceBeforeUppercase(type.ToString()).ToUpper()}: {message}", debugParam.Color);
                }
            }
            else
            {
                _colorLog(message, Color.white);
            }
        }

        public static void Log(object invoker, string message, Type type = Type.None)
        {
            DebugParam debugParam = _params.FirstOrDefault(d => d.Type == type);
            
            if (debugParam != null)
            {
                if (debugParam.Active)
                {
                    _colorLog(
                        $"{_insertSpaceBeforeUppercase(type.ToString()).ToUpper()} {invoker.GetType().Name}: {message}",
                        debugParam.Color);
                }
            }
            else
            {
                _colorLog(message, Color.white);
            }
        }

        public static void LogError(object obj, string message)
        {
            _colorLog($"{obj.GetType()} {message}", Color.red);
        }

        public static void LogError(string message)
        {
            _colorLog(message, Color.red);
        }

        private static void _colorLog(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }

        private static string _insertSpaceBeforeUppercase(string input)
        {
            StringBuilder result = new();

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