using System;
using Code.Data.Enums;

namespace Code.Data.Value
{
    [Serializable]
    public class LiveStatePercentageValue
    {
        public LiveStateKey Key;
        public float Value;
    }
}