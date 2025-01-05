using System;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;

namespace Code.Data.Value
{
    [Serializable]
    public struct LiveStateRangePercentageValue
    {
        public ELiveStateKey Key;
        [MinMaxRangeFloat(-1, 1)] public RangedFloat PercentageValue;
    }
}