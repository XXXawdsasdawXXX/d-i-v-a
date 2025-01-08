using System;

namespace Code.Data
{
    public interface IToggle
    {
        void Active(Action OnTurnedOn = null);
        void Disable(Action onTurnedOff = null);
    }
}