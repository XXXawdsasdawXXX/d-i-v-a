using UnityEngine;

namespace Code.Data.Audio
{
    public abstract class AudioEvent : ScriptableObject
    {
        public EAudioEventType Type;
        public abstract void Play(AudioSource source);
    }
}