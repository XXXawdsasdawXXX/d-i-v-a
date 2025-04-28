using Code.Data;
using UnityEngine;

namespace Code.Game.Services.Time
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Configs/Time config")]
    public class TimeConfig : ScriptableObject
    {
        [SerializeField, MinMaxRangeFloat(5, 6000)]
        public RangedFloat TickRangedTime;
        [Space]
        public float ClickSeries = 0.75f;
        [Space]
        public CooldownData Cooldown;
        [Space]
        public DurationData Duration;
        [Space]
        public DelayTickData Delay;
    }
}