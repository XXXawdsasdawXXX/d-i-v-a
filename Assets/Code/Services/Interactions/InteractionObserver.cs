
using System;

namespace Code.Services
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