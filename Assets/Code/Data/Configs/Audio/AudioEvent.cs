using Code.Data.Enums;
using UnityEngine;

namespace Code.Data.Configs.Audio
{
    public abstract class AudioEvent : ScriptableObject
    {
        public EAudioEventType Type;
        public abstract void Play(AudioSource source);
    }
}