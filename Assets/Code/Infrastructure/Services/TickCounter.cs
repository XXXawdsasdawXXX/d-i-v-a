using System;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.Services
{
    [Serializable]
    public class TickCounter : IToggle
    {
        private readonly TimeObserver _timeObserver;

        private int _tickCount;
        private int _currentTickNumber;

        private bool _isLoop;
        private bool _isActive;

        public bool IsExpectedStart { get; private set; } = true;
        
        public event Action OnWaitIsOver;

        #region Constructors

        public TickCounter(bool isLoop = true)
        {
            _isLoop = isLoop;
            _timeObserver = Container.Instance?.FindService<TimeObserver>();
            SubscribeToEvents(true);
        }

        public TickCounter(int tickCount, bool isLoop = true)
        {
            _isLoop = isLoop;
            _tickCount = tickCount;
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            SubscribeToEvents(false);
        }

        #endregion

        #region Methods

        public int GetRemainingTick()
        {
            return _tickCount > 0 && !IsExpectedStart ? _tickCount - _currentTickNumber : 0;
        }

        public void StartWait()
        {
            _currentTickNumber = 0;
            if (IsExpectedStart && _tickCount > 0)
            {
                IsExpectedStart = false;
                On();
            }
        }

        public void StartWait(int count, Action onStartWait = null)
        {
            if (!IsExpectedStart)
            {
                StopWait();
            }

            if (IsExpectedStart && count > 0)
            {
                IsExpectedStart = false;
                _tickCount = count;
                onStartWait?.Invoke();
                On();
            }
        }

        public void StopWait(bool isStopLoop = false)
        {
            _currentTickNumber = 0;
            IsExpectedStart = true;

            if (!_isLoop || (_isLoop && isStopLoop))
            {
                Off();
            }
        }

        public void On(Action onTurnedOn = null)
        {
            _isActive = true;
        }

        public void Off(Action onTurnedOff = null)
        {
            _isActive = false;
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
            if (IsExpectedStart || !_isActive)
            {
                return;
            }

            _currentTickNumber++;
            if (_currentTickNumber >= _tickCount)
            {
                OnWaitIsOver?.Invoke();
                StopWait();
            }
        }

        #endregion
    }
}