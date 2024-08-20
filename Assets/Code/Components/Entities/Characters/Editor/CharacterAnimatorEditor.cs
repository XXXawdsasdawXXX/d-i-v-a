using UnityEditor;
using UnityEngine;

namespace Code.Components.Entities.Characters.Editor
{
    [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CharacterAnimator characterAnimator = (CharacterAnimator)target;

            //Mode
            if (GUILayout.Button("Empty")) characterAnimator.SetEmptyMode();

            if (GUILayout.Button("Stand")) characterAnimator.SetStandMode();

            if (GUILayout.Button("Seat")) characterAnimator.SetSeatMode();

            if (GUILayout.Button("Sleep")) characterAnimator.SetSleepMode();

            //State

            if (GUILayout.Button("Start eat")) characterAnimator.StartPlayEat();

            if (GUILayout.Button("Stop eat")) characterAnimator.StopPlayEat();

            if (GUILayout.Button("Reaction voice")) characterAnimator.PlayReactionVoice();

            if (GUILayout.Button("Start reaction mouse")) characterAnimator.StartPlayReactionMouse();

            if (GUILayout.Button("Stop reaction mouse")) characterAnimator.StopPlayReactionMouse();
        }
    }
}