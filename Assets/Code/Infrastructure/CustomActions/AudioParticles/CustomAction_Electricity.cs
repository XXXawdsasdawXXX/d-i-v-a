using Code.Data.Enums;
using Code.Services.LoopbackAudio.Audio;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Electricity: CustomAction_AudioParticle
    {
        private  LoopbackAudioService _loopbackAudioService;

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Electricity;
        }
        
        protected override ParticleType GetParticleType()
        {
            return ParticleType.Electricity;
        }

        protected override void UpdateParticles()
        {
            if (_particlesSystems == null)
            {
                return;
            }
            foreach (var particle in _particlesSystems)
            {
                particle.transform.position = _diva.transform.position;
            }
        }
    }
}