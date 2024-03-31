using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
	public  AudioEventType Type;
	public abstract void Play(AudioSource source);
}