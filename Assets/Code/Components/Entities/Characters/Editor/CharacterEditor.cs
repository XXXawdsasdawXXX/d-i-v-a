using UnityEditor;
using UnityEngine;

namespace Code.Components.Entities.Characters.Editor
{
    [CustomEditor(typeof(DIVA))]
    public class CharacterEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DIVA diva = (DIVA)target;

            if (GUILayout.Button("Find all components"))
            {
                diva.FindAllComponents();
                EditorUtility.SetDirty(diva);
            }
        }
    }
}