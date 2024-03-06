using Code.Components.Objects;
using UnityEditor;
using UnityEngine;

namespace Code.Components.Characters.Editor
{
    [CustomEditor(typeof(MaterialController))]
    public class MaterialControllerEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MaterialController materialController = (MaterialController)target;

            if (GUILayout.Button("Set active snine"))
            {
                materialController.SetActiveShine();
            }
        }
    }
}