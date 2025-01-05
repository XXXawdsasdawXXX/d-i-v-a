using UnityEditor;
using UnityEngine;

namespace Code.Components.Entities.Editor
{
    [CustomEditor(typeof(Grass))]
    public class GrassEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Grass grass = (Grass)target;

            if (GUILayout.Button("Find All Components"))
            {
                grass.FindAllComponents();
                EditorUtility.SetDirty(grass);
            }
        }
    }
}