using UnityEditor;
using UnityEngine;

namespace Code.Entities.Grass.Editor
{
    [CustomEditor(typeof(GrassEntity))]
    public class GrassEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GrassEntity grass = (GrassEntity)target;

            if (GUILayout.Button("Find All Components"))
            {
                grass.FindAllComponents();
                EditorUtility.SetDirty(grass);
            }
        }
    }
}