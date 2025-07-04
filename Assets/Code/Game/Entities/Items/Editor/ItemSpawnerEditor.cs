﻿using UnityEditor;
using UnityEngine;

namespace Code.Game.Entities.Items.Editor
{
    [CustomEditor(typeof(ItemSpawner))]
    public class ItemSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ItemSpawner spawner = (ItemSpawner)target;

            if (GUILayout.Button("Spawn Random Item"))
            {
                spawner.SpawnRandomItem();
            }
        }
    }
}