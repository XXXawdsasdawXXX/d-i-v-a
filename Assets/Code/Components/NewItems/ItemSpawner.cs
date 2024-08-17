using Code.Data.Interfaces;
using Code.Infrastructure.DI;
using Code.Infrastructure.GameLoop;
using Code.Services.Pools;
using UnityEngine;

namespace Code.Components.NewItems
{
    public class ItemSpawner: MonoBehaviour, IService, IGameInitListener
    {
        [SerializeField] private MonoPool<Item> _monoPool;
        
        [Header("Services")]
        private ItemDataService _itemDataService;

        public void GameInit()
        {
            _itemDataService = Container.Instance.FindService<ItemDataService>();
        }
        
        public Item SpawnRandomItem(Transform anchor)
        {
           var item =  _monoPool.GetNext();
           item.SetData(_itemDataService.GetRandomItemData(), anchor);
           return item;
        }
    }
}