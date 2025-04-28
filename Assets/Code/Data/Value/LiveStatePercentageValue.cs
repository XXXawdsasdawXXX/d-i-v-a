using System;
using Code.Game.Services.LiveState;
using UnityEngine;

namespace Code.Data
{
    [Serializable]
    public class LiveStatePercentageValue
    {
        public ELiveStateKey Key;
        [Range(-1, 1)] public float Value;
    }
}