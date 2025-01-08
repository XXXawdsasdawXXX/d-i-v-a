using System;

namespace Code.Data
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