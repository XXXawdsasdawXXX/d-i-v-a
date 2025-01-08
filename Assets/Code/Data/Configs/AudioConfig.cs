using System.Linq;
using Code.Data.Audio;
using UnityEngine;

namespace Code.Data
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/Audio config")]
    public class AudioConfig : ScriptableObject
    {
        public AudioEvent[] AudioEvents;

        public AudioEvent[] GetAudioEvents(EAudioEventType type)
        {
            return AudioEvents.Where(a => a.Type == type).ToArray();
        }

        public AudioEvent GetRandomAudioEvent(EAudioEventType type)
        {
            AudioEvent[] array = GetAudioEvents(type);
            return array[Random.Range(0, array.Length)];
        }
    }
}