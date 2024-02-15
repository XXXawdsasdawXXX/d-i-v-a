using Code.Data.StaticData;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AppleConfig", menuName = "Configs/Apple")]
    public class AppleConfig : ScriptableObject
    {
        [Header("Time for spawn apple branch")]
        [MinMaxRangeFloat(1, 300)] public RangedFloat SpawnCooldownMinutes;
        [Header("Time for one apple stage")]
        [MinMaxRangeFloat(60, 600)] public RangedFloat LiveTimeSecond;
        [Header("Apples stage params\nValue is a percentage of the state's maximum value.")]
        public LiveStateValues[] SmallAppleValues;
        public LiveStateValues[] BigAppleValues;

    }
}