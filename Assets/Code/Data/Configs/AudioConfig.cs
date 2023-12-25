using Code.Data.StaticData;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio config")]
    public class AudioConfig : ScriptableObject
    {
        public MicrophoneAnalyzerData MicrophoneAnalyzerData;
    }
}