using Code.Data.StaticData;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "TimeConfig", menuName = "Configs/Time config")]
    public class TimeConfig : ScriptableObject
    {
        [SerializeField,MinMaxRangeFloat(5,6000)] 
        public RangedFloat TickRangedTime;
        public float ClickSeries = 0.75f;
        public CooldownTickData Cooldown;
        public DurationTickData Duration;
    }
}