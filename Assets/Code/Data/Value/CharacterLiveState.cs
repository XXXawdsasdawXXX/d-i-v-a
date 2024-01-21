using System;

namespace Code.Data.Value
{
    [Serializable]
    public class CharacterLiveState
    {
        private  float _current;
        private float _max;
        
        private float _decreasingValue;
        private readonly float _healValue;
        public float Current => _current;
        public float Max => _max;

        public float GetPercent() => _max == 0 ? 0 :_current / _max;

        public event Action<float> ChangedEvent;
        
        public CharacterLiveState(float current,float max, float decreasingValue, float healValue)
        {
            _current = current;
            _max = max;
            _decreasingValue = decreasingValue;
            _healValue = healValue;
        }

        public void Heal()
        {
            _current += _decreasingValue;
            if (_current > _max)
            {
                _current = _max;
            }
            ChangedEvent?.Invoke(_current);
        }

        public void TimeUpdate()
        {
            _current -= _decreasingValue;
            if (_current < 0)
            {
                _current = 0;
            }
            ChangedEvent?.Invoke(_current);
        }

        public void Add(float value)
        {
            _current += value;
            if (_current > _max)
            {
                _current= _max;
            }
            ChangedEvent?.Invoke(_current);
        }

        public void Remove(float value)
        {
            _current -= value;
            if (_current < 0)
            {
                _current = 0;
            }
            ChangedEvent?.Invoke(_current);
        }
    }
}