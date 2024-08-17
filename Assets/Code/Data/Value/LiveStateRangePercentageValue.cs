using System;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;

namespace Code.Data.Value
{
    [Serializable]
    public class LiveStateRangePercentageValue
    {
        public LiveStateKey Key;
        [MinMaxRangeFloat(-1, 1)] public RangedFloat PercentageValue;
    }
}