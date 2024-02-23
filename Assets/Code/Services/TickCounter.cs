using System;
using Code.Infrastructure.DI;

namespace Code.Services
{
    [Serializable]
    public class TickCounter 
    {
        private readonly TimeObserver _timeObserver;
        
        private int _tickCount;
        private int _currentTickNumber;

        private bool _isLoop;
        public bool IsWaited { get; private set; } = true;
        public event Action WaitedEvent;
        
        #region Constructors

        public TickCounter(bool isLoop = true)
        {
            _isLoop = isLoop;
            _timeObserver = Container.Instance?.FindService<TimeObserver>();
        }
        
        public TickCounter(int tickCount,bool isLoop = true)
        {
            _isLoop = isLoop;
            _tickCount = tickCount;
            _timeObserver = Container.Instance.FindService<TimeObserver>();
        }
        
        #endregion

        #region Methods

        public int GetRemainingTick()
        {
            return _tickCount > 0 ? _tickCount - _currentTickNumber : 0;
        }
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

        public void StopWait(bool isStopLoop = false)
        {
            _currentTickNumber = 0;
            IsWaited = true;
           
            if (!_isLoop || (_isLoop && isStopLoop))
            {
                SubscribeToEvents(false);
            }
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
                WaitedEvent?.Invoke();
                StopWait(); 
            }
        }

        #endregion
    }
}