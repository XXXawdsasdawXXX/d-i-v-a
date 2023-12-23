﻿using UnityEditor;
using UnityEngine;

namespace Code.Components.Character.Editor
{
        [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterAnimatorEditor : UnityEditor.Editor
    {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                CharacterAnimator characterAnimator = (CharacterAnimator)target;
            
                if (GUILayout.Button("Stand"))
                {
                    characterAnimator.SetStandMode();
                }

                if (GUILayout.Button("Seat"))
                {
                    characterAnimator.SetSeatMode();
                }
                
                if (GUILayout.Button("Sleep"))
                {
                    characterAnimator.SetSleepMode();
                }
            }
        
    }
}