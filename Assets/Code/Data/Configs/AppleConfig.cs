using Code.Data.Value;
using Code.Data.Value.RangeInt;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AppleConfig", menuName = "Configs/Apple")]
    public class AppleConfig : ScriptableObject
    {
        [Header("Time for spawn apple branch")] [MinMaxRangeInt(1, 100)]
        public RangedInt SpawnCooldownTick;

        [Header("Time for one apple stage")] [MinMaxRangeInt(1, 100)]
        public RangedInt OneStageLiveTimeTick;

        [Header("Apples stage params\nValue is a percentage of the state's maximum value.")]
        public LiveStatePercentageValues[] AppleValues;
    }
}