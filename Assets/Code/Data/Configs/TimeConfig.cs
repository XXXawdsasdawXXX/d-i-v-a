using Code.Data.StaticData;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Configs/Time config")]
    public class TimeConfig : ScriptableObject
    {
        [SerializeField, MinMaxRangeFloat(5, 6000)]
        public RangedFloat TickRangedTime;
        [Space]
        public float ClickSeries = 0.75f;
        [Space]
        public CooldownTickData Cooldown;
        [Space]
        public DurationData Duration;
        [Space]
        public DelayTickData Delay;
    }
}