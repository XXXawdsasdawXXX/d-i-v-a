using Code.Components.Character.Params;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Configs/Character config")]
    public class CharacterConfig : ScriptableObject
    {
        public DecreasingValue EatValue;
        public DecreasingValue SleepValue;
        public DecreasingValue FearValue;
    }
}