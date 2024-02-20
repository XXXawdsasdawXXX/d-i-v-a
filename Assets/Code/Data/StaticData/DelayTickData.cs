using System;
using Code.Data.Value.RangeFloat;

namespace Code.Data.StaticData
{
    [Serializable]
    public class DelayTickData
    {
        [MinMaxRangeInt(1,100)]public RangedInt GrassGrow;
    }
}