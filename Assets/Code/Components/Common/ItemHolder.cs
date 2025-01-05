using Code.Components.Items;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Components.Common
{
    public class ItemHolder : CommonComponent, IGameUpdateListener
    {
        [Header("Static value")] 
        [SerializeField] private Vector2 _offset;

        [field: SerializeField] public Item SelectedItem { get; private set; }

        public void GameUpdate()
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
            Item item = SelectedItem;
            SelectedItem = null;
            return item;
        }
    }
}