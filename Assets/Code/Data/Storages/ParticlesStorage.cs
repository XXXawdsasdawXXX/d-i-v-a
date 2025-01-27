using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.DI;
using Code.Infrastructure.Factories;
using Code.Infrastructure.GameLoop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Data
{
    public class ParticlesStorage : MonoBehaviour, IStorage, IInitListener
    {
        [SerializeField] private List<ParticleSystemFacade> _particles;

        private VFXConfig _vfxConfig;
        private ParticleFactory _factory;

        public UniTask GameInitialize()
        {
            _factory = Container.Instance.FindService<ParticleFactory>();
            
            return UniTask.CompletedTask;
        }

        public bool TryGetParticle(EParticleType particleType, out ParticleSystemFacade[] particles)
        {
            particles = _particles.Where(p => p.Type == particleType).ToArray();

            if (particles.Length == 0)
            {
                particles = _factory.CreateParticles(particleType, transform, Vector3.zero).ToArray();
             
                _particles.AddRange(particles);
            }

            return particles != null && particles.Length > 0;
        }

        [ContextMenu("InitializeSceneParticles")]
        public void InitializeSceneParticles()
        {
            ParticleSystemFacade[] allParticles = GetComponentsInChildren<ParticleSystemFacade>();
            
            _particles.Clear();
            
            _particles = allParticles.ToList();
        }

        public bool TryGetParticles(IEnumerable<EParticleType> particleTypes, out ParticleSystemFacade[] particles)
        {
            List<ParticleSystemFacade> list = new();
            
            foreach (EParticleType particleType in particleTypes)
            {
                if (TryGetParticle(particleType, out ParticleSystemFacade[] typedParticles))
                {
                    list.AddRange(typedParticles);
                }
            }

            particles = list.ToArray();
            
            return particles.Length > 0;
        }
    }
}