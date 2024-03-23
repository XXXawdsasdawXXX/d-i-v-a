using System;
using Code.Data.Value.RangeFloat;
using Code.Data.Value.RangeInt;

namespace Code.Data.StaticData
{
    [Serializable]
    public class DelayTickData
    {
        [MinMaxRangeInt(1,100)]public RangedInt GrassGrow;
    }
}