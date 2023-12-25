using System;
using Code.Data.Value.RangeFloat;

namespace Code.Data.StaticData
{
    [Serializable]
    public class MicrophoneAnalyzerData
    {
        [MinMaxRange(-50, 0)] public RangedFloat MinActionDecibels;        // -12/-20 - min  
        [MinMaxRange(-50, 0)] public RangedFloat MaxActionDecibels;        //-3 - max  
    }
}