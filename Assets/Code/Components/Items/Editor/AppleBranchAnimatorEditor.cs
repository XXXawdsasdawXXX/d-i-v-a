using UnityEditor;
using UnityEngine;

namespace Code.Components.Apples.Editor
{
    [CustomEditor(typeof(AppleBranchAnimator))]
    public class AppleBranchAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AppleBranchAnimator appleBranchAnimator = (AppleBranchAnimator)target;
            
            if(GUILayout.Button("Play Enter")) appleBranchAnimator.PlayEnter();
            if(GUILayout.Button("Play Exit")) appleBranchAnimator.PlayExit();
        }
    }

    [CustomEditor(typeof(AppleBranch))]
    public class AppleBranchEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AppleBranch appleBranch = (AppleBranch)target;

            if(GUILayout.Button("Grow brunch"))  appleBranch.GrowBranch();
        }
    }
}