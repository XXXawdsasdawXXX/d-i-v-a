using System.Collections.Generic;
using System.Linq;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Factories
{
    public class AssetsFactory : IService, IInitListener
    {
        private VFXConfig _vfxConfig;

        public void GameInitialize()
        {
            _vfxConfig = Container.Instance.FindConfig<VFXConfig>();
        }

        public IEnumerable<ParticleSystemFacade> CreateParticles(EParticleType type, Transform root, Vector3 position)
        {
            ParticleSystemFacade[] particles = _vfxConfig.GetParticles(type)
                .Select(particleSystem => CreateParticle(particleSystem, root, position)).ToArray();
            return particles;
        }

        public ParticleSystemFacade CreateParticle(EParticleType type, Transform root, Vector3 position)
        {
            return CreateParticle(_vfxConfig.GetParticle(type), root, position);
        }

        private ParticleSystemFacade CreateParticle(ParticleSystemFacade prefab, Transform root, Vector3 position)
        {
            ParticleSystemFacade particle = Object.Instantiate(prefab, position, prefab.transform.rotation);
            particle.transform.SetParent(root);
            particle.GameInitialize();
            if (!particle.gameObject.activeSelf)
            {
                particle.gameObject.SetActive(true);
            }

            return particle;
        }
    }
}