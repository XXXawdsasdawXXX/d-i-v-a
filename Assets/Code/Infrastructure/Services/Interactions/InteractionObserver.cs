using System;

namespace Code.Infrastructure.Services.Interactions
{
    public abstract class InteractionObserver
    {
        public event Action OnAddedInteraction;

        protected void InvokeInteractionEvent()
        {
            OnAddedInteraction?.Invoke();
        }
    }
}