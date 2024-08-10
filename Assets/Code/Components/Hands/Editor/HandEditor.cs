using UnityEditor;
using UnityEngine;

namespace Code.Components.Hands.Editor
{
    [CustomEditor(typeof(Hand))]
    public class HandEditor : UnityEditor.Editor
    {
        
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Hand hand = (Hand)target;

        if (GUILayout.Button("Find all components"))
        {
            hand.FindAllComponents();
        }
    }
    }
}