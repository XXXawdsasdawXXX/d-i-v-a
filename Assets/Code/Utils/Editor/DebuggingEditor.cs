using UnityEditor;
using UnityEngine;

namespace Code.Utils.Editor
{
    [CustomEditor(typeof(Log))]
    public class DebuggingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Log log = (Log)target;

            if (GUILayout.Button("Disable All"))
            {
                log.DisableAll();
            }
        }
    }
}