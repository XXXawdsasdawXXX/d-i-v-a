﻿using Code.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Data.Audio
{
    [CreateAssetMenu(menuName = "Configs/Audio Events/Simple", fileName = "AudioEvent_")]
    public class SimpleAudioEvent : AudioEvent
    {
        public AudioClip[] clips;
        public RangedFloat volume;

        [MinMaxRangeFloat(0, 2)] public RangedFloat pitch;

        public override void Play(AudioSource source)
        {
            if (clips.Length == 0) return;
            source.clip = clips[Random.Range(0, clips.Length)];
            Debugging.Log($"Play audio {source.clip.name}");
            source.volume = Random.Range(volume.MinValue, volume.MaxValue);
            source.pitch = Random.Range(pitch.MinValue, pitch.MaxValue);
            source.Play();
        }
    }
}