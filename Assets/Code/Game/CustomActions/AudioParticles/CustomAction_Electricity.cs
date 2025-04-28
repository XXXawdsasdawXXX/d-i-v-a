using Code.Game.Effects;
using Code.Infrastructure.LoopbackAudio;

namespace Code.Game.CustomActions.AudioParticles
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