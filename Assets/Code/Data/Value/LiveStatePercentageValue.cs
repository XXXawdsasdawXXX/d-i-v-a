using System;
using Code.Data.Enums;
using UnityEngine;

namespace Code.Data.Value
{
    [Serializable]
    public class LiveStatePercentageValue
    {
        public ELiveStateKey Key;
        [Range(-1, 1)] public float Value;
    }
}