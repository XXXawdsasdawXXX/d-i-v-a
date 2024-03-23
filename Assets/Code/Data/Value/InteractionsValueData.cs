using System;
using Code.Data.Value.RangeInt;

namespace Code.Data.StaticData
{
    [Serializable]
    public class InteractionsValueData
    {
        [MinMaxRangeInt(0, 200)] public RangedInt InteractionsCount;
        [MinMaxRangeInt(0, 100)] public RangedInt Value;
    }
}