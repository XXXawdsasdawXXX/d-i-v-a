using UnityEditor;
using UnityEngine;

namespace Code.Components.Entities.Editor
{
    [CustomEditor(typeof(Diva))]
    public class CharacterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Diva diva = (Diva)target;

            if (GUILayout.Button("Find all components"))
            {
                diva.FindAllComponents();
                EditorUtility.SetDirty(diva);
            }
        }
    }
}