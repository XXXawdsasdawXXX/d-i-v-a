using System;
using Code.Data;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.Services
{
    [Serializable]
    public class TickCounter : IToggle
    {
        public event Action OnWaitIsOver;
        public bool IsExpectedStart { get; private set; } = true;

        private readonly TimeObserver _timeObserver;

        private int _tickCount;
        private int _currentTickNumber;

        private bool _isLoop;
        private bool _isActive;


        public TickCounter(bool isLoop = true)
        {
            _isLoop = isLoop;
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _subscribeToEvents(true);
        }

        public TickCounter(int tickCount, bool isLoop = true)
        {
            _isLoop = isLoop;
            _tickCount = tickCount;
            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _subscribeToEvents(false);
        }

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
                Active();
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
                Active();
            }
        }

        public void StopWait(bool isStopLoop = false)
        {
            _currentTickNumber = 0;
            IsExpectedStart = true;

            if (!_isLoop || (_isLoop && isStopLoop))
            {
                Disable();
            }
        }

        public void Active(Action OnTurnedOn = null)
        {
            _isActive = true;
        }

        public void Disable(Action onTurnedOff = null)
        {
            _isActive = false;
        }

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _timeObserver.TickEvent += _onTick;
            }
            else
            {
                _timeObserver.TickEvent -= _onTick;
            }
        }

        private void _onTick()
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
    }
}