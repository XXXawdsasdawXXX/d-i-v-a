using UnityEditor;
using UnityEngine;

namespace Code.Game.Entities.Common.Editor
{
    [CustomEditor(typeof(MaterialAdapter)), CanEditMultipleObjects]
    public class MaterialControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MaterialAdapter materialAdapter = (MaterialAdapter)target;

            if (GUILayout.Button("Refresh"))
            {
                materialAdapter.Editor_RefreshState();
                materialAdapter.Editor_RefreshValue();
            }
        }
    }
}