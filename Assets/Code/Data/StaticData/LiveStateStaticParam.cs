using System;
using Code.Data.Enums;

namespace Code.Data.StaticData
{
    [Serializable]
    public class LiveStateStaticParam
    {
        public ELiveStateKey Key;
        public float DecreasingValue = 0.1f;
        public float MaxValue = 10;
        public float HealValue = 2;
    }
}