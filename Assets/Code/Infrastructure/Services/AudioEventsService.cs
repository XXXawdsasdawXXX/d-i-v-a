using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

public class AudioEventsService : MonoBehaviour, IService, IGameInitListener
{
    [SerializeField] private AudioSource _audioSource;
    private AudioConfig _config;

    void IGameInitListener.GameInit()
    {
        _config = Container.Instance.FindConfig<AudioConfig>();
    }

    public void PlayAudio(AudioEventType type)
    {
        var audio = _config.GetRandomAudioEvent(type);
        if(audio != null)
        {
            audio.Play(_audioSource);
        }
    }
}