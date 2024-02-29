using System;

namespace Code.Infrastructure.CustomActions
{
    public abstract class CustomAction
    {
        public bool IsActive { get; protected set; }
      
        public Action<CustomAction> EndCustomActionEvent;
        public abstract void StartAction();
        public abstract void StopAction();
        public abstract CustomCutsceneActionType GetActionType();
    }
}