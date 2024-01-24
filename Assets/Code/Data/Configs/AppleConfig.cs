using Code.Data.StaticData;
using Code.Data.Value.RangeFloat;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "AppleConfig", menuName = "Configs/Apple")]
    public class AppleConfig : ScriptableObject
    {
        [MinMaxRange(1, 300)] public RangedFloat SpawnCooldownMinutes;
        [MinMaxRange(120, 300)] public RangedFloat LiveTimeSecond;
        [Header("Apples stage params\nValue is a percentage of the state's maximum value.")]
        public LiveStateValues[] SmallAppleValues;
        public LiveStateValues[] BigAppleValues;

    }
}