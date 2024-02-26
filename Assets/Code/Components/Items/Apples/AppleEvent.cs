using System;

namespace Code.Components.Apples
{
    public class AppleEvent
    {
        public event Action SetBigAppleEvent;
        public void InvokeSetBigAppleEvent() => SetBigAppleEvent?.Invoke();
        
        public event Action UseEvent;
        public void InvokeUseEvent() => UseEvent?.Invoke();

        public event Action StartIllEvent;
        public void InvokeStartIllEvent() => StartIllEvent?.Invoke();
        
        public event Action DieEvent;
        public void InvokeDieEvent() => DieEvent?.Invoke();
    }
}