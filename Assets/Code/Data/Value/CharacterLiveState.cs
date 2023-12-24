using System;
using Code.Data.Enums;

namespace Code.Data.Value
{
    [Serializable]
    public class CharacterLiveState
    {
            
        public LiveStateKey _key;
        public float _current;
        public float _max;
        
        private float _decreasingValue;
        public LiveStateKey GetLiveStateKey() => _key;
        public float GetPercent() => _max == 0 ? 0 :_current / _max;

        public event Action<float> OnChanged;


        public CharacterLiveState(LiveStateKey key,float current,float max, float decreasingValue)
        {
            _key = key;
            _current = current;
            _max = max;
            _decreasingValue = decreasingValue;
        }
        
        public void TimeUpdate()
        {
            _current -= _decreasingValue;
            if (_current < 0)
            {
                _current = 0;
            }
            OnChanged?.Invoke(_current);
        }
        
        public void Add(float value)
        {
            _current += value;
            if (_current > _max)
            {
                _current= _max;
            }
            OnChanged?.Invoke(_current);
        }

        public void Remove(float value)
        {
            _current -= value;
            if (_current < 0)
            {
                _current = 0;
            }
            OnChanged?.Invoke(_current);
        }

    }
}