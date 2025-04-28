using System;
using Code.Game.Effects;

namespace Code.Data
{
    [Serializable]
    public class ParticleData
    {
        public EParticleType Type;
        public ParticleSystemFacade[] Objects;
    }
}