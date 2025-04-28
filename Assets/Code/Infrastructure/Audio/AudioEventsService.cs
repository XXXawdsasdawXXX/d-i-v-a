using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Audio
{
    public class AudioEventsService : MonoBehaviour, IService, IInitializeListener
    {
        [SerializeField] private AudioSource _audioSource;
      
        private AudioConfig _config;

        public UniTask GameInitialize()
        {
            _config = Container.Instance.GetConfig<AudioConfig>();
            
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