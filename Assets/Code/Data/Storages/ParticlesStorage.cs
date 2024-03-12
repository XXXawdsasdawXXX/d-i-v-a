using System;
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
        [SerializeField] private ParticleData[] _particles;

        private VFXConfig _vfxConfig;
        private AssetsFactory _factory;


        public void GameInit()
        {
            _factory = Container.Instance.FindService<AssetsFactory>();
        }

        public bool TryGetParticle(ParticleType particleType, out ParticleSystemFacade[] particleSystem)
        {
            var data = _particles.FirstOrDefault(p => p.Type == particleType);
            
            if (data == null)
            {
                data = new ParticleData()
                {
                    Objects = _factory.CreateParticles(particleType, transform, Vector3.zero).ToArray(),
                    Type = particleType
                };
            }
            else if(data.Objects.Length == 0)
            {
                data.Objects = _factory.CreateParticles(particleType, transform, Vector3.zero).ToArray();
            }
            
            particleSystem = data?.Objects;
            return particleSystem != null && particleSystem.Length > 0;
        }
    }
}