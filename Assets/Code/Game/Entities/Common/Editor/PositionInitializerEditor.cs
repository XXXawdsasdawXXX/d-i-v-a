﻿using UnityEditor;
using UnityEngine;

namespace Code.Game.Entities.Common.Editor
{
    [CustomEditor(typeof(PositionInitializer))]
    public class PositionInitializerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PositionInitializer positionInitializer = (PositionInitializer)target;
            if (GUILayout.Button("set position")) positionInitializer.SetDefaultPosition();
        }
    }
}