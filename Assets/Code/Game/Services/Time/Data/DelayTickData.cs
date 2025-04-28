using System;
using Code.Data;

namespace Code.Game.Services.Time
{
    [Serializable]
    public class DelayTickData
    {
        [MinMaxRangeInt(1, 100)] public RangedInt GrassGrow;
    }
}