using System;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio config")]
    public class AudioConfig : ScriptableObject
    {
        public MicrophoneAnalyzerData MicrophoneAnalyzerData;
    }


    [Serializable]
    public class MicrophoneAnalyzerData
    {
        [MinMaxRange(-50, 0)] public RangedFloat MinActionDecibels;        // -12/-20 - min  
        [MinMaxRange(-50, 0)] public RangedFloat MaxActionDecibels;        //-3 - max  
    }
}