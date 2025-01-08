using UnityEditor;
using UnityEngine;

namespace Code.Entities.Diva.Editor
{
    [CustomEditor(typeof(DivaEntity))]
    public class DivaEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
         
            DivaEntity diva = (DivaEntity)target;

            if (GUILayout.Button("Find all components"))
            {
                diva.FindAllComponents();
            
                EditorUtility.SetDirty(diva);
            }
        }
    }
}