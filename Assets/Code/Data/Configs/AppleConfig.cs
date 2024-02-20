using Code.Data.StaticData;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AppleConfig", menuName = "Configs/Apple")]
    public class AppleConfig : ScriptableObject
    {
        [Header("Time for spawn apple branch")]
        [MinMaxRangeFloat(1, 100)] public RangedInt SpawnCooldownTick;
        [Header("Time for one apple stage")]
        [MinMaxRangeFloat(1, 100)] public RangedInt OneStageLiveTimeTick;
        [Header("Apples stage params\nValue is a percentage of the state's maximum value.")]
        public LiveStateValues[] SmallAppleValues;
        public LiveStateValues[] BigAppleValues;

    }
}