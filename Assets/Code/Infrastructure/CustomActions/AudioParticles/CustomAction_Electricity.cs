using Code.Data.Enums;
using Code.Data.VFX;
using Code.Infrastructure.Services.LoopbackAudio.Audio;
using Code.Utils;

namespace Code.Infrastructure.CustomActions.AudioParticles
{
    public class CustomAction_Electricity : CustomAction_AudioParticle
    {
        private LoopbackAudioService _loopbackAudioService;

        public override CustomCutsceneActionType GetActionType()
        {
            return CustomCutsceneActionType.Electricity;
        }

        protected override ParticleType[] GetParticleTypes()
        {
            return new[] { ParticleType.Electricity };
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