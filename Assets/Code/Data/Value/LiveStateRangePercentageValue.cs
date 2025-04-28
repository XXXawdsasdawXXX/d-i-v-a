using System;
using Code.Game.Services.LiveState;

namespace Code.Data
{
    [Serializable]
    public struct LiveStateRangePercentageValue
    {
        public ELiveStateKey Key;
        [MinMaxRangeFloat(-1, 1)] public RangedFloat PercentageValue;
    }
}