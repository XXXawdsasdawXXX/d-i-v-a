using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Save;
using UnityEngine;

namespace Code.Infrastructure.CustomActions
{
    public class CustomAction_Sakura : CustomAction, IGameInitListener,IProgressWriter
    {
        public void GameInit()
        {
            
        }

        protected override void StartAction()
        {
            
        }

        protected override void StopAction()
        {
            
        }

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Sakura;
        }

        public void LoadProgress(PlayerProgressData playerProgress)
        {
            
        }

        public void UpdateProgress(PlayerProgressData playerProgress)
        {
            
        }
    }
}