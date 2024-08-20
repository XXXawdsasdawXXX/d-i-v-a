using System;

namespace Code.Infrastructure.Services.Interactions
{
    public abstract class InteractionObserver
    {
        public event Action InteractionEvent;

        protected void InvokeInteractionEvent()
        {
            InteractionEvent?.Invoke();
        }
    }
}