using System;
using Code.Utils;

namespace Code.Data.Value
{
    [Serializable]
    public class CharacterLiveState
    {
        public event Action<float> OnChanged;
        
        public float Max { get; }
        public float Current { get; private set; }
        public bool IsHealing { get; private set; }
        public float GetPercent() => Max == 0 ? 0 : Current / Max;

        private readonly float _healValue;
        
        private float _decreasingValue;

        public CharacterLiveState(float current, float max, float decreasingValue, float healValue, bool isHealing)
        {
            Current = current;
            Max = max;
            IsHealing = isHealing;

            _decreasingValue = decreasingValue;
            _healValue = healValue;
        }

        public void TimeUpdate()
        {
            if (IsHealing)
            {
                Add(_healValue);
            }
            else
            {
                Remove(_decreasingValue);
            }
        }

        public void Add(float value)
        {
            Current += value;
         
            if (Current > Max)
            {
                Current = Max;
            }
            else if (Current < 0)
            {
                Current = 0;
            }

            OnChanged?.Invoke(Current);
        }

        public void Remove(float value)
        {
            Current -= value;
            if (Current < 0)
            {
                Current = 0;
            }

            OnChanged?.Invoke(Current);
        }

        public void SetHealUpdate()
        {
            IsHealing = true;
        }

        public void SetDefaultUpdate()
        {
            IsHealing = false;
        }
    }
}