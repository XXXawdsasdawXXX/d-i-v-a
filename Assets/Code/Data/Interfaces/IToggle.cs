using System;

namespace Code.Data.Interfaces
{
    public interface IToggle
    {
        void On(Action onTurnedOn = null);
        void Off(Action onTurnedOff = null);
    }
}