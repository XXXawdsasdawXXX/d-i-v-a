using System;

namespace Code.Data.Interfaces
{
    public interface IToggle
    {
        void Active(Action OnTurnedOn = null);
        void Disable(Action onTurnedOff = null);
    }
}