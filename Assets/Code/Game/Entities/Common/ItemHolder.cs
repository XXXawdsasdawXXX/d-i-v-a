using Code.Game.Entities.Items;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using UnityEngine;

namespace Code.Game.Entities.Common
{
    public class ItemHolder : CommonComponent, IUpdateListener
    {
        [Header("Static value")] 
        [SerializeField] private Vector2 _offset;

        [field: SerializeField] public ItemEntity SelectedItem { get; private set; }

        public void GameUpdate()
        {
            if (SelectedItem != null)
            {
                SelectedItem.transform.position = transform.position + _offset.AsVector3();
            }
        }

        public void SetItem(ItemEntity item)
        {
            SelectedItem = item;
        }

        public ItemEntity DropItem()
        {
            ItemEntity item = SelectedItem;
        
            SelectedItem = null;
            
            return item;
        }
    }
}