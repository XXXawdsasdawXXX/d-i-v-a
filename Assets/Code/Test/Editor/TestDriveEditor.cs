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
            
            if (GUILayout.Button("Move next point"))
            {
                testDrive.MoveToNextPosition();
            }
        }
        
    }
}