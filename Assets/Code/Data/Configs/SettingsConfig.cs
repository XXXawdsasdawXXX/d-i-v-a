using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "SettingsConfig", menuName = "Configs/SettingsConfig")]
    public class SettingsConfig : ScriptableObject
    {
        public byte ColorCheckSensitivity = 8;
    }
}