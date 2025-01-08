using System;

namespace Code.Data
{
    [Serializable]
    public class DelayTickData
    {
        [MinMaxRangeInt(1, 100)] public RangedInt GrassGrow;
    }
}