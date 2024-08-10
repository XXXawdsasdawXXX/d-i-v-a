using System.Collections.Generic;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Interfaces;
using Code.Data.VFX;
using Code.Infrastructure.DI;
using Code.Infrastructure.Factories;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Data.Storages
{
    public class ParticlesStorage : MonoBehaviour, IStorage, IGameInitListener
    {
        [SerializeField] private List<ParticleSystemFacade> _particles;

        private VFXConfig _vfxConfig;
        private AssetsFactory _factory;
        
        public void GameInit()
        {
            _factory = Container.Instance.FindService<AssetsFactory>();
        }

        public bool TryGetParticle(ParticleType particleType, out ParticleSystemFacade[] particleSystem)
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
            var allParticles = GetComponentsInChildren<ParticleSystemFacade>();
            _particles.Clear();
            _particles = allParticles.ToList();
        }

        public bool TryGetParticles(IEnumerable<ParticleType> particleTypes, out ParticleSystemFacade[] particlesSystems)
        {
            var list = new List<ParticleSystemFacade>();
            foreach (var particleType in particleTypes) 
            {
                if (TryGetParticle(particleType, out var typedParticles))
                {
                    list.AddRange(typedParticles);
                }
            }

            particlesSystems = list.ToArray();
            return particlesSystems.Length > 0;
        }
    }
}