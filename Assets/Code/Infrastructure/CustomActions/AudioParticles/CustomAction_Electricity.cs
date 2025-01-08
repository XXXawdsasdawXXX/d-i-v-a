using Code.Data;
using Code.Infrastructure.Services.LoopbackAudio.Audio;
using Code.Utils;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Electricity : CustomAction_AudioParticle
    {
        private LoopbackAudioService _loopbackAudioService;

        public override ECustomCutsceneActionType GetActionType()
        {
            return ECustomCutsceneActionType.Electricity;
        }

        protected override EParticleType[] GetParticleTypes()
        {
            return new[] { EParticleType.Electricity };
        }

        protected override void UpdateParticles()
        {
            if (_particlesSystems == null)
            {
                return;
            }

            foreach (ParticleSystemFacade particle in _particlesSystems)
            {
                particle.transform.position = _diva.transform.position;
            }
        }
    }
}