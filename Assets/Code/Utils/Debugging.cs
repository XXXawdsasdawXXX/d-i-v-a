using System;
using System.Linq;
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
            Time
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
            if (debugParam is { Active: true })
            {
                ColorLog(message, debugParam.Color);
            }
        }

        public void TestLog(string message)
        {
            ColorLog(message,Color.green);
        }
        private void ColorLog(string message, Color color)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>" + message + "</color>");
        }
    }
}