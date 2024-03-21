using System;

namespace Code.Infrastructure.CustomActions
{
    public abstract class CustomAction
    {
        public bool IsActive { get; private set; }

        protected virtual void StartAction()
        {
            IsActive = true;
        }

        protected virtual void StopAction()
        {
            IsActive = false;
        }
        public abstract CustomCutsceneActionType GetActionType();
        
        public Action<CustomAction> EndCustomActionEvent;
    }
}