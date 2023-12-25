using UnityEditor;
using UnityEngine;

namespace Code.Components.Character.Editor
{
    [CustomEditor(typeof(CharacterLiveStateTimer))]
    public class CharacterLiveStateControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CharacterLiveStateTimer stateTimerEditor = (CharacterLiveStateTimer)target;

            if (GUILayout.Button("Log"))
            {
                stateTimerEditor.LogStates();
            }
        }
    }
}