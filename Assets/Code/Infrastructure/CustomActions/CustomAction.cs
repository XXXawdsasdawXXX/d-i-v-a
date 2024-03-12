using System;

namespace Code.Infrastructure.CustomActions
{
    public abstract class CustomAction
    {
        public bool IsActive { get; protected set; }
        protected abstract void StartAction();
        protected  abstract void StopAction();
        public abstract CustomCutsceneActionType GetActionType();
        
        public Action<CustomAction> EndCustomActionEvent;
    }
}