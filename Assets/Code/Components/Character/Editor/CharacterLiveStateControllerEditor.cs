using UnityEditor;
using UnityEngine;

namespace Code.Components.Character.Editor
{
    [CustomEditor(typeof(CharacterLiveStateController))]
    public class CharacterLiveStateControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CharacterLiveStateController stateControllerEditor = (CharacterLiveStateController)target;

            if (GUILayout.Button("Log"))
            {
                stateControllerEditor.LogStates();
            }
        }
    }
}