using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Code.Utils
{
    public class Log : MonoBehaviour
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
            Input,
            Audio
        }

        [Serializable]
        private struct DebugParam
        {
            public Type Type;
            public bool Active;
            public Color Color;
        }

        [SerializeField] private DebugParam[] _debugParams;

        private static readonly Dictionary<Type, DebugParam> _params = new();

        private void Awake()
        {
            foreach (DebugParam param in _debugParams)
            {
                _params.Add(param.Type, param);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), System.Diagnostics.Conditional("LOG")]
        public static void Info(string message, Type type = Type.None)
        {
            DebugParam debugParam = _params[type];

            if (debugParam.Active)
            {
                _colorLog($"{_insertSpaceBeforeUppercase(type.ToString()).ToUpper()}: {message}", debugParam.Color);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), System.Diagnostics.Conditional("LOG")]
        public static void Info(object invoker, string message, Type type = Type.None)
        {
            DebugParam debugParam = _params[type];

            if (debugParam.Active)
            {
                _colorLog(
                    $"{_insertSpaceBeforeUppercase(type.ToString()).ToUpper()} {invoker.GetType().Name}: {message}",
                    debugParam.Color);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), System.Diagnostics.Conditional("LOG")]
        public static void Error(object obj, string message)
        {
            _colorLog($"{obj.GetType()} {message}", Color.red);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), System.Diagnostics.Conditional("LOG")]
        public static void Error(string message)
        {
            _colorLog(message, Color.red);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), System.Diagnostics.Conditional("LOG")]
        private static void _colorLog(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            for (int i = 0; i < _debugParams.Length; i++)
            {
                DebugParam debugParam = _debugParams[i];

                debugParam.Active = false;

                _debugParams[i] = debugParam;
            }

            EditorUtility.SetDirty(this);
        }

        private void OnValidate()
        {
            for (int i = 0; i < _debugParams.Length; i++)
            {
                DebugParam debugParam = _debugParams[i];

                if (debugParam.Color == new Color())
                {
                    debugParam.Color = Color.white;
                }

                _debugParams[i] = debugParam;
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}