using UnityEditor;
using UnityEngine;

namespace Code.Entities.Items.Editor
{
    [CustomEditor(typeof(ItemEntity))]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ItemEntity item = (ItemEntity)target;

            if (GUILayout.Button("Find all components"))
            {
                item.FindAllComponents();
            }
        }
    }
}