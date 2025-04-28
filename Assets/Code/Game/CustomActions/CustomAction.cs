using System;

namespace Code.Game.CustomActions
{
    public abstract class CustomAction
    {
        public Action<CustomAction> EndCustomActionEvent;
        public bool IsActive { get; protected set; }
        
        
        public abstract ECustomCutsceneActionType GetActionType();
        
        protected virtual void TryStartAction()
        {
            IsActive = true;
        }

        protected virtual void StopAction()
        {
            IsActive = false;
        }
    }
}