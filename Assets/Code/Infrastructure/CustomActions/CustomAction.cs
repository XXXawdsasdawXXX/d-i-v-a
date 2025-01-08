using System;
using Code.Data;

namespace Code.Infrastructure.CustomActions
{
    public abstract class CustomAction
    {
        public bool IsActive { get; protected set; }

        protected virtual void TryStartAction()
        {
            IsActive = true;
        }

        protected virtual void StopAction()
        {
            IsActive = false;
        }

        public abstract ECustomCutsceneActionType GetActionType();

        public Action<CustomAction> EndCustomActionEvent;
    }
}