﻿using Code.Components.Objects;
using UnityEditor;
using UnityEngine;

namespace Code.Components.Characters.Editor
{
    [CustomEditor(typeof(MaterialController)),CanEditMultipleObjects]
    public class MaterialControllerEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MaterialController materialController = (MaterialController)target;

            if (GUILayout.Button("Refresh"))
            {
                materialController.Editor_RefreshState();
                materialController.Editor_RefreshValue();
            }


        }
    }
    
    
   
}