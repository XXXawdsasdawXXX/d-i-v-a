using Code.Components.Items;
using UnityEditor;
using UnityEngine;

namespace Code.Components.Apples.Editor
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Item item = (Item)target;

            if (GUILayout.Button("Find all components"))
            {
                item.FindAllComponents();
            }
        }
    }
}