using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace Code.Game.Effects
{
    [Preserve]
    public class ParticleFactory : IService, IInitializeListener
    {
        private VFXConfig _vfxConfig;
     
        private GameEventDispatcher _gamEventDispatcher;
        private Spawner _spawner;

        public UniTask GameInitialize()
        {
            _vfxConfig = Container.Instance.GetConfig<VFXConfig>();
            _spawner = Container.Instance.GetService<Spawner>();
            
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
            ParticleSystemFacade particle = _spawner.Instantiate(prefab, position, prefab.transform.rotation);
      
            particle.transform.SetParent(root);

            if (!particle.gameObject.activeSelf)
            {
                particle.gameObject.SetActive(true);
            }

            return particle;
        }
    }
}