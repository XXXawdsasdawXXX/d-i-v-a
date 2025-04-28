using UnityEngine;

namespace Code.Infrastructure.Audio
{
    public abstract class AudioEvent : ScriptableObject
    {
        public EAudioEventType Type;
        public abstract void Play(AudioSource source);
    }
}