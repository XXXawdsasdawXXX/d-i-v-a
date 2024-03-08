using Code.Data.Facades;
using Code.Infrastructure.GameLoop;
using UnityEngine;

namespace Code.Test
{
    public class TestParticleFacade : MonoBehaviour
    {
        private enum AudioType
        {
            None,
            ScaledMax,
            ScaledEnergy
        }

        private enum ParamType
        {
            None,
        }
        
        [SerializeField] private ParticleSystemFacade _particleSystem;
    }
}