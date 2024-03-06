using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ReactiveEffects.Editor
{
    [CustomEditor(typeof(PrefabLayoutAudioObject)), CanEditMultipleObjects]
    public class PrefabEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PrefabLayoutAudioObject prefab = (PrefabLayoutAudioObject)target;
            
            if(GUILayout.Button("refresh")) prefab.Refresh();
        }
    }

    [CustomEditor(typeof(MaterialColorIntensityReactiveEffect)),CanEditMultipleObjects]
    public class MaterialColorIntensityReactiveEffectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            MaterialColorIntensityReactiveEffect effect = (MaterialColorIntensityReactiveEffect)target;
            
            
         

        }
    }
}