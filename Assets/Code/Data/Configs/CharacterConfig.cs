using Code.Components.Character.Params;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Configs/Character config")]
    public class CharacterConfig : ScriptableObject
    {
        public DecreasingLiveStateValue[] DecreasingLiveStateValues;
    }
}