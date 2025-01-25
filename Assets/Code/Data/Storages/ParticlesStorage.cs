using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.DI;
using Code.Infrastructure.Factories;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Data
{
    public class ParticlesStorage : MonoBehaviour, IStorage, IInitListener
    {
        [SerializeField] private List<ParticleSystemFacade> _particles;

        private VFXConfig _vfxConfig;
        private AssetsFactory _factory;

        public void GameInitialize()
        {
            _factory = Container.Instance.FindService<AssetsFactory>();
        }

        public bool TryGetParticle(EParticleType particleType, out ParticleSystemFacade[] particleSystem)
        {
            particleSystem = _particles.Where(p => p.Type == particleType).ToArray();

            if (particleSystem.Length == 0)
            {
                particleSystem = _factory.CreateParticles(particleType, transform, Vector3.zero).ToArray();
                _particles.AddRange(particleSystem);
            }

            return particleSystem != null && particleSystem.Length > 0;
        }

        [ContextMenu("Init")]
        public void InitSceneParticles()
        {
            ParticleSystemFacade[] allParticles = GetComponentsInChildren<ParticleSystemFacade>();
            _particles.Clear();
            _particles = allParticles.ToList();
        }

        public bool TryGetParticles(IEnumerable<EParticleType> particleTypes,
            out ParticleSystemFacade[] particlesSystems)
        {
            List<ParticleSystemFacade> list = new();
            foreach (EParticleType particleType in particleTypes)
            {
                if (TryGetParticle(particleType, out ParticleSystemFacade[] typedParticles))
                {
                    list.AddRange(typedParticles);
                }
            }

            particlesSystems = list.ToArray();
            return particlesSystems.Length > 0;
        }
    }
}