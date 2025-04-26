using System;
using System.Linq;
using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Utils;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace Code.Entities.Items
{
    public class ItemDataService : IService, IInitListener
    {
        private ItemData[] _itemsData;

        public UniTask GameInitialize()
        {
            _itemsData = Container.Instance.FindConfig<ItemsConfig>().Items;
            
            return UniTask.CompletedTask;
        }

        public ItemData GetRandomItemData(ItemType itemType = ItemType.None)
        {
            if (itemType == ItemType.None)
            {
                itemType = _getRandomType();
            }

            ItemData[] items = _itemsData.Where(i => i.Type == itemType).ToArray();
           
            Extensions.ShuffleArray(items);

            foreach (ItemData itemData in items)
            {
                int randomChance = Random.Range(0, 100);
            
                if (itemData.SpawnChance > randomChance)
                {
                    Log.Info(this, $"(chance {itemData.SpawnChance} >= {randomChance}) " +
                                        $"return {itemData.Type} {itemData.AnimatorController.name}", 
                        Log.Type.Items);

                    return itemData;
                }

#if DEBUGGING
                Log.Info(this, $"(chance {itemData.SpawnChance} >= {randomChance})", Log.Type.Items);
#endif
            }

            return items[Random.Range(0, items.Length - 1)];
        }

        private ItemType _getRandomType()
        {
            ItemType[] types = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray();
           
            ItemType randomType = types[Random.Range(1, types.Length - 1)];
            
            return randomType;
        }
    }
}