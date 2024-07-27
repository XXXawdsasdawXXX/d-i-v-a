using Code.Components.Items;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class ItemHolder : CommonComponent, IGameTickListener
    {
        [Header("Static value")] 
        [SerializeField] private Vector2 _offset;
        [field: SerializeField] public Item SelectedItem { get; private set; }

        public void GameTick()
        {
            if (SelectedItem != null)
            {
                SelectedItem.transform.position = transform.position + _offset.AsVector3();
            }
        }

        public void SetItem(Item item)
        {
            SelectedItem = item;
        }

        public Item DropItem()
        {
            var item = SelectedItem;
            SelectedItem = null;
            return item;
        }
    }
}