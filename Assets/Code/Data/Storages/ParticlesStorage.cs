using System;
using System.Collections.Generic;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Facades;
using Code.Data.Interfaces;
using Code.Data.StaticData;
using Code.Infrastructure.DI;
using Code.Infrastructure.Factories;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Services
{
    public class ParticlesStorage : MonoBehaviour, IService, IGameInitListener
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
    }
}