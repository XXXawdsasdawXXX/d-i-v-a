using Code.Infrastructure.GameLoop;
using Code.Infrastructure.Pools;
using Code.Infrastructure.ServiceLocator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Game.Entities.Items
{
    public class ItemSpawner: MonoBehaviour, IService, IInitializeListener
    {
        [SerializeField] private MonoPool<ItemEntity> _monoPool;
        
        [Header("Services")]
        private ItemDataService _itemDataService;

        public UniTask GameInitialize()
        {
            _itemDataService = Container.Instance.GetService<ItemDataService>();
            
            return UniTask.CompletedTask;
        }
        
        public ItemEntity SpawnRandomItem()
        {
           ItemEntity item =  _monoPool.GetNext();
           
           item.SetData(_itemDataService.GetRandomItemData());
           
           return item;
        }
    }
}