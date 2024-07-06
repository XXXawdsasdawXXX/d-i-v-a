using UnityEditor;
using UnityEngine;

namespace Code.Test.Editor
{
    [CustomEditor(typeof(TestDrive))]
    public class TestDriveEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TestDrive testDrive = (TestDrive)target;
            
            if (GUILayout.Button("Set time scale"))
            {
                testDrive.SetTimeScale();
            }
        }
    }
    
    [CustomEditor(typeof(LiveStateValidator))]
    public class LiveStateValidatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            LiveStateValidator validator = (LiveStateValidator)target;
            
            if (GUILayout.Button("Add value"))
            {
                validator.ChangeState();
            }
            
            if (GUILayout.Button("Debug lower key"))
            {
                validator.DebugLowerState();
            }
        }
    }
}