using System.Collections.Generic;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.VFX;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Infrastructure.Factories
{
    public class AssetsFactory : IService, IGameInitListener
    {
        private VFXConfig _vfxConfig;

        public void GameInit()
        {
            _vfxConfig = Container.Instance.FindConfig<VFXConfig>();
        }

        public IEnumerable<ParticleSystemFacade> CreateParticles(ParticleType type, Transform root, Vector3 position)
        {
            var particles = _vfxConfig.GetParticles(type)
                .Select(particleSystem => CreateParticle(particleSystem, root, position)).ToArray();
            return particles;
        }

        public ParticleSystemFacade CreateParticle(ParticleType type, Transform root, Vector3 position)
        {
            return CreateParticle(_vfxConfig.GetParticle(type), root, position);
        }

        private ParticleSystemFacade CreateParticle(ParticleSystemFacade prefab, Transform root, Vector3 position)
        {
            var particle = Object.Instantiate(prefab, position, prefab.transform.rotation);
            particle.transform.SetParent(root);
            particle.GameInit();
            if (!particle.gameObject.activeSelf)
            {
                particle.gameObject.SetActive(true);
            }

            return particle;
        }
    }
}