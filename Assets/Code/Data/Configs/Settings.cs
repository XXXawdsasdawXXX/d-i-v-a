using UnityEngine;

namespace Code.Data
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Configs/Settings")]
    public class Settings : ScriptableObject
    {
        public byte ColorCheckSensitivity = 8;
    }
}