using Code.Components.Items;
using UnityEngine;

namespace Code.Data.Configs
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "Configs/Items Config")]
    public class ItemsConfig :ScriptableObject
    {
        public ItemData[] Items;
    }
}