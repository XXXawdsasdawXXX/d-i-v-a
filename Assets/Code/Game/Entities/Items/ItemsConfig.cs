using UnityEngine;

namespace Code.Game.Entities.Items
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName = "Configs/Items Config")]
    public class ItemsConfig :ScriptableObject
    {
        public ItemData[] Items;
    }
}