using System.Linq;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using UnityEngine;

namespace Code.Services
{
    public class ParticlesDictionary : MonoBehaviour, IService
    {
        [SerializeField] private ParticleData[] _particles;

        public bool TryGetParticle(ParticleType particleType, out ParticleSystem particleSystem)
        {
            particleSystem = _particles.FirstOrDefault(p => p.Type == particleType)?.Object;
            return particleSystem != null;
        }
    }
}