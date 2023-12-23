using System;

namespace Code.Components.Character.Params
{
    [Serializable]
    public class CharacterLiveState
    {
        private float _current;
        private float _max;
        public LiveStateKey Key { get; private set; }

        public float Percent => _max == 0 ? 0 :_current / _max;
        
        public event Action<float> OnChanged;
        

        public void SetParams(LiveStateKey key, float current, float max)
        {
            Key = key;
            _current = current;
            _max = max;
        }
        
        public void Add(float value)
        {
            _current += value;
            if (_current > _max)
            {
                _current = _max;
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