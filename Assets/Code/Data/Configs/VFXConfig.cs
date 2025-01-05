using System.Collections.Generic;
using System.Linq;
using Code.Data.Enums;
using Code.Data.VFX;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "VFXConfig", menuName = "Configs/VFXConfig")]
    public class VFXConfig : ScriptableObject
    {
        [SerializeField] private ParticleSystemFacade[] _allParticles;

        public ParticleSystemFacade GetParticle(EParticleType particleType)
        {
            return _allParticles.FirstOrDefault(p => p.Type == particleType);
        }

        public IEnumerable<ParticleSystemFacade> GetParticles(EParticleType particleType)
        {
            return _allParticles.Where(p => p.Type == particleType);
        }
    }
}