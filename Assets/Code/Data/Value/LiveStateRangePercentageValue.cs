using System;

namespace Code.Data
{
    [Serializable]
    public struct LiveStateRangePercentageValue
    {
        public ELiveStateKey Key;
        [MinMaxRangeFloat(-1, 1)] public RangedFloat PercentageValue;
    }
}