using System;
using Code.Data.Value.RangeFloat;

namespace Code.Data.StaticData
{
    [Serializable]
    public class MicrophoneAnalyzerData
    {
        [MinMaxRangeFloat(-50, 0)] public RangedFloat MinActionDecibels; // -12/-20 - min  
        [MinMaxRangeFloat(-50, 0)] public RangedFloat MaxActionDecibels; //-3 - max  
    }
}