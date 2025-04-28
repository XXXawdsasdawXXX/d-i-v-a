using System;
using Code.Data;
using UnityEngine;

namespace Code.Game.Services.Time
{
    [Serializable]
    public class DurationData
    {
        [Header("Tick")] 
        [Header("d i v a")]
        [MinMaxRangeInt(1, 100)] public RangedInt LookToMouse;
        [MinMaxRangeInt(1, 100)] public RangedInt Stand; 
        public int StoppingTicksToMaximumSleepValues = 7;

        [Space]
        [Header("Seconds")] 
        public float StarryMouse = 1;
        [Header("user")] 
        public float UserStandStillSecond = 120;
    }
}