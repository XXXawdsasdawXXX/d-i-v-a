using UnityEditor;
using UnityEngine;

namespace Code.Character.Editor
{
        [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterAnimatorEditor : UnityEditor.Editor
    {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                CharacterAnimator testDrive = (CharacterAnimator)target;
            
                if (GUILayout.Button("Stand"))
                {
                    testDrive.PlayStand();
                }

                if (GUILayout.Button("Seat"))
                {
                    testDrive.PlaySeat();
                }
            }
        
    }
}