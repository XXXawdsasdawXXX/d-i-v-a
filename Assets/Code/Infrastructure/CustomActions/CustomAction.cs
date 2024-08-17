using System;
using Code.Data.Enums;

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

        public abstract CustomCutsceneActionType GetActionType();

        public Action<CustomAction> EndCustomActionEvent;
    }
}