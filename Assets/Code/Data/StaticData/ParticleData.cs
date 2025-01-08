using System;

namespace Code.Data
{
    [Serializable]
    public class ParticleData
    {
        public EParticleType Type;
        public ParticleSystemFacade[] Objects;
    }
}