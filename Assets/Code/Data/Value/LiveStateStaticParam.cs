using System;
using Code.Data.Enums;

namespace Code.Data.Value
{
    [Serializable]
    public class LiveStateStaticParam
    {
        public LiveStateKey Key;
        public float DecreasingValue = 0.1f;
        public float MaxValue = 10;
    }
}