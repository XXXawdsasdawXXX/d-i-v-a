using System;
using Code.Data.Enums;
using Code.Data.VFX;

namespace Code.Data.StaticData
{
    [Serializable]
    public class ParticleData
    {
        public EParticleType Type;
        public ParticleSystemFacade[] Objects;
    }
}