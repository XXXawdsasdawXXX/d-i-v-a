using System.Linq;
using Code.Data.Configs.Audio;
using Code.Data.Enums;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio config")]
    public class AudioConfig : ScriptableObject
    {
        public AudioEvent[] AudioEvents;

        public AudioEvent[] GetAudioEvents(AudioEventType type)
        {
            return AudioEvents.Where(a => a.Type == type).ToArray();
        }

        public AudioEvent GetRandomAudioEvent(AudioEventType type)
        {
            AudioEvent[] array = GetAudioEvents(type);
            return array[Random.Range(0, array.Length)];
        }
    }
}