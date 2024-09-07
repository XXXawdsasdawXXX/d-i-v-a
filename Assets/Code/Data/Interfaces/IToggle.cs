using System;

namespace Code.Data.Interfaces
{
    public interface IToggle
    {
        void On(Action OnTurnedOn = null);
        void Off(Action onTurnedOff = null);
    }
}