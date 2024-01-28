using System;

namespace Code.Components.Apples
{
    public class AppleEvent
    {
        public event Action EndLiveTimeEvent;
        public void InvokeEndLiveTimeEvent() => EndLiveTimeEvent?.Invoke();
        
        public event Action StartIllEvent;
        public void InvokeStartIllEvent() => StartIllEvent?.Invoke();
        
        
        public event Action GrowEvent;
        public void InvokeGrowEvent() => GrowEvent?.Invoke();
    }
}