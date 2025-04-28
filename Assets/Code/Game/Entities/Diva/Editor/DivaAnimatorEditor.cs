using UnityEditor;
using UnityEngine;

namespace Code.Game.Entities.Diva.Editor
{
    [CustomEditor(typeof(DivaAnimator))]
    public class DivaAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DivaAnimator divaAnimator = (DivaAnimator)target;

            //Mode
            if (GUILayout.Button("Empty")) divaAnimator.SetEmptyMode();

            if (GUILayout.Button("Stand")) divaAnimator.SetStandMode();

            if (GUILayout.Button("Seat")) divaAnimator.SetSeatMode();

            if (GUILayout.Button("Sleep")) divaAnimator.SetSleepMode();

            //State

            if (GUILayout.Button("Start eat")) divaAnimator.StartPlayEat();

            if (GUILayout.Button("Stop eat")) divaAnimator.StopPlayEat();

            if (GUILayout.Button("Reaction voice")) divaAnimator.PlayReactionVoice();

            if (GUILayout.Button("Start reaction mouse")) divaAnimator.StartPlayReactionMouse();

            if (GUILayout.Button("Stop reaction mouse")) divaAnimator.StopPlayReactionMouse();
            
            if (GUILayout.Button("Start hide hand")) divaAnimator.PlayHideHand();
            
            if (GUILayout.Button("Stop hide hand")) divaAnimator.PlayShowHand();
        }
    }
}