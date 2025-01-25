using Code.Data;
using Code.Data.Audio;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class AudioEventsService : MonoBehaviour, IService, IInitListener
    {
        [SerializeField] private AudioSource _audioSource;
        private AudioConfig _config;

        void IInitListener.GameInitialize()
        {
            _config = Container.Instance.FindConfig<AudioConfig>();
        }

        public void PlayAudio(EAudioEventType type)
        {
            AudioEvent audio = _config.GetRandomAudioEvent(type);
            if (audio != null)
            {
                audio.Play(_audioSource);
            }
        }
    }
}