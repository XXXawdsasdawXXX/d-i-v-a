using Code.Data;
using Code.Data.Audio;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Services
{
    public class AudioEventsService : MonoBehaviour, IService, IInitListener
    {
        [SerializeField] private AudioSource _audioSource;
      
        private AudioConfig _config;

        public UniTask GameInitialize()
        {
            _config = Container.Instance.FindConfig<AudioConfig>();
            
            return UniTask.CompletedTask;
        }

        public void PlayAudio(EAudioEventType type)
        {
            AudioEvent audioEvent = _config.GetRandomAudioEvent(type);
         
            if (audioEvent != null)
            {
                audioEvent.Play(_audioSource);
            }
        }
    }
}