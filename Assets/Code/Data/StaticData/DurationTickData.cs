using System;
using Code.Data.Value.RangeFloat;

namespace Code.Data.StaticData
{
    [Serializable]
    public class DurationTickData
    {
        [MinMaxRangeInt(1, 100)] public RangedInt LookToMouse;
        [MinMaxRangeInt(1,100)] public RangedInt Stand;
        public float StarryMouse = 1;
    }
}