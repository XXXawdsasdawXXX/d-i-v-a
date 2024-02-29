using System;
using Code.Data.Enums;
using UnityEngine;

namespace Code.Data.StaticData
{
    [Serializable]
    public class ParticleData
    {
        public ParticleType Type;
        public ParticleSystem Object;
    }
}