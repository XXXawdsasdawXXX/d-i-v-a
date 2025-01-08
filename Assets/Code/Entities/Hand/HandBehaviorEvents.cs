using System;

namespace Code.Entities.Hand
{
    public class HandBehaviorEvents : HandComponent
    {
        public event Action OnWillAppear;
        public event Action OnHidden;

        public void InvokeWillAppear()
        {
            OnWillAppear?.Invoke();
        }

        public void InvokeHidden()
        {
            OnHidden?.Invoke();
        }
    }
}