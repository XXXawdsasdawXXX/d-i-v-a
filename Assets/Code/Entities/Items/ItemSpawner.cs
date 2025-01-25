using Code.Data;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Pools;
using UnityEngine;

namespace Code.Entities.Items
{
    public class ItemSpawner: MonoBehaviour, IService, IInitListener
    {
        [SerializeField] private MonoPool<ItemEntity> _monoPool;
        
        [Header("Services")]
        private ItemDataService _itemDataService;

        public void GameInitialize()
        {
            _itemDataService = Container.Instance.FindService<ItemDataService>();
        }
        
        public ItemEntity SpawnRandomItem(Transform anchor)
        {
           ItemEntity item =  _monoPool.GetNext();
           item.SetData(_itemDataService.GetRandomItemData());
           return item;
        }
    }
}