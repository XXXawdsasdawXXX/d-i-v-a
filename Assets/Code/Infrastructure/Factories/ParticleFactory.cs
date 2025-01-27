using System.Collections.Generic;
using System.Linq;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Infrastructure.Factories
{
    [Preserve]
    public class ParticleFactory : IService, IInitListener
    {
        private VFXConfig _vfxConfig;

        public UniTask GameInitialize()
        {
            _vfxConfig = Container.Instance.FindConfig<VFXConfig>();
            
            return UniTask.CompletedTask;
        }

        public IEnumerable<ParticleSystemFacade> CreateParticles(EParticleType type, Transform root, Vector3 position)
        {
            ParticleSystemFacade[] particles = _vfxConfig.GetParticles(type)
                .Select(particleSystem => _createParticle(particleSystem, root, position)).ToArray();
           
            return particles;
        }

        public ParticleSystemFacade CreateParticle(EParticleType type, Transform root, Vector3 position)
        {
            return _createParticle(_vfxConfig.GetParticle(type), root, position);
        }

        private ParticleSystemFacade _createParticle(ParticleSystemFacade prefab, Transform root, Vector3 position)
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