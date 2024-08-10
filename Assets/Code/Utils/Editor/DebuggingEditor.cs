using UnityEditor;
using UnityEngine;

namespace Code.Utils.Editor
{
    [CustomEditor(typeof(Debugging))]
    public class DebuggingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Debugging debugging = (Debugging)target;

            if (GUILayout.Button("Disable All"))
            {
                debugging.DisableAll();
            }
        }
    }
}