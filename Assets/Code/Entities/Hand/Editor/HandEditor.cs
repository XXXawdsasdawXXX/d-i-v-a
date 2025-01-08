using UnityEditor;
using UnityEngine;

namespace Code.Entities.Hand.Editor
{
    [CustomEditor(typeof(HandEntity))]
    public class HandEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HandEntity hand = (HandEntity)target;

            if (GUILayout.Button("Find all components"))
            {
                hand.FindAllComponents();
            }
        }
    }
    
    [CustomEditor(typeof(HandAnimator))]
    public class HandAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HandAnimator hand = (HandAnimator)target;

            if (GUILayout.Button("Enter"))
            {
                hand.PlayEnterHand();
            }
            if (GUILayout.Button("Exit"))
            {
                hand.PlayExitHand();
            }
        }
    }
}