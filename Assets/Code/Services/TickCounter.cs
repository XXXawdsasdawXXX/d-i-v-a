using System;
using Code.Infrastructure.DI;

namespace Code.Services
{
    public class TickCounter 
    {
        private readonly TimeObserver _timeObserver;
        
        private int _tickCount;
        private int _currentTickNumber;
        public bool IsWaited { get; private set; } = true;

        public event Action WaitedEvent;
        
        #region Constructors

        public TickCounter()
        {
            _timeObserver = Container.Instance.FindService<TimeObserver>();
        }
        
        public TickCounter(int tickCount)
        {
            _tickCount = tickCount;
            _timeObserver = Container.Instance.FindService<TimeObserver>();
        }
        
        #endregion

        #region Methods

        public void StartWait()
        {
            if (IsWaited && _tickCount > 0)
            {
                IsWaited = false;
                SubscribeToEvents(true);
            }
        }

        public void StartWait(int count)
        {
            if (IsWaited && count > 0)
            {
                IsWaited = false;
                _tickCount = count;
                SubscribeToEvents(true);
            }
        }

        public void StopWait()
        {
            IsWaited = true;
            SubscribeToEvents(false);
        }

        #endregion

        #region Events

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.TickEvent += OnTick;
            }
            else
            {
                _timeObserver.TickEvent -= OnTick;
            }
        }

        private void OnTick()
        {
            _currentTickNumber++;
            if (_currentTickNumber >= _tickCount)
            {
                IsWaited = true;
                WaitedEvent?.Invoke();
            }
        }

        #endregion
    }
}