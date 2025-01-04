using System;
using System.Linq;
using Code.Data.Configs;
using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Random = UnityEngine.Random;

namespace Code.Components.Items
{
    public class ItemDataService: IService, IGameInitListener
    {
        private ItemData[] _itemsData;

        public void GameInit()
        {
            _itemsData = Container.Instance.FindConfig<ItemsConfig>().Items;
        }
        
        public ItemData GetRandomItemData(ItemType itemType = ItemType.None)
        {
            if (itemType == ItemType.None)
            {
                itemType = GetRandomType();
            }
            
            //var items = _itemsData.Where(i => i.Type == itemType).ToArray();
            ItemData[] items = _itemsData;//todo восстановить, когда будет больше итемов
            Extensions.ShuffleArray(items);
            
            foreach (ItemData itemData in items)
            {
                int randomChance = Random.Range(0, 100);
                if (itemData.SpawnChance > randomChance)
                {
                    Debugging.Instance.Log(this,$"(chance {itemData.SpawnChance} >= {randomChance})" +
                                                $"return {itemData.Type} {itemData.AnimatorController.name}"
                        ,Debugging.Type.Items);
                    return itemData;
                }
                else
                {
                    
                    Debugging.Instance.Log(this,$"(chance {itemData.SpawnChance} >= {randomChance})"
                        ,Debugging.Type.Items);
                }
            }
            
            return items[Random.Range(0, items.Length - 1)];
        }

        private  ItemType GetRandomType()
        {
            ItemType[] types = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray();
            ItemType randomType = types[Random.Range(1, types.Length - 1)];
            return randomType;
        }
    }
}