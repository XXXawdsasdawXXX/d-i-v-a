using UnityEditor;
using UnityEngine;

namespace Code.Components.Characters.Editor
{
    [CustomEditor(typeof(Character))]
    public class CharacterEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Character character = (Character)target;

            if (GUILayout.Button("Find all components"))
            {
                character.FindAllComponents();
            }
        }
    }
}