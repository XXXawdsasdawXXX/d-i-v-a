using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Components.Entities.Hands;
using Code.Components.Items;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand.Behavior
{
    public class BehaviourNode_ShowItem : BaseNode
    {
        [Header("d i v a")] 
        private readonly Transform _divaTransform;

        [Header("Hand")] 
        private readonly Components.Entities.Hands.Hand _hand;
        private readonly HandAnimator _handAnimation;
        private readonly MovementToMouse _movementToMouse;
        private readonly ItemHolder _itemHolder;

        [Header("Item")] 
        private readonly ItemSpawner _itemSpawner;
        private Item _item;

        [Header("Services")] 
        private readonly PositionService _positionService;

        [Header("Dynamic data")] 
        private ItemData _currentItemData;
        private Vector3 _spawnPosition;


        public BehaviourNode_ShowItem()
        {
            //d i v a
            _divaTransform = Container.Instance.FindEntity<DIVA>().transform;
            
            //hand
            _hand = Container.Instance.FindEntity<Components.Entities.Hands.Hand>();
            _handAnimation = _hand.FindHandComponent<HandAnimator>();
            _movementToMouse = _hand.FindCommonComponent<MovementToMouse>();
            _itemHolder = _hand.FindCommonComponent<ItemHolder>();
            
            //item
            _itemSpawner = Container.Instance.FindService<ItemSpawner>();
            
            //services
            _positionService = Container.Instance.FindService<PositionService>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _hand.transform.position = _spawnPosition;
                _handAnimation.PlayEnterHand(onEndEnter: () =>
                {
                    _item = _itemSpawner.SpawnRandomItem(anchor: _hand.transform);
                    _itemHolder.SetItem(_item);
                    _movementToMouse.On();
                    SubscribeToEvents(true);
                });
                
                return;
                
            }

            Return(false);
        }

        private void HideHand()
        {
            SubscribeToEvents(false);
            
            _movementToMouse.Off();
            _itemHolder.DropItem();
            _handAnimation.PlayExitHand(() =>
            {
                Return(true);
            });
        }
        
        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _item.OnDestroyed += HideHand;
                _item.OnPressed += HideHand;
                
            }
            else
            {
                _item.OnDestroyed -= HideHand;
                _item.OnPressed -= HideHand;
            }
        }
        
        protected override bool IsCanRun()
        {
            return _positionService
                .TryGetRandomDistantPosition(targetPosition: _divaTransform.position, minDistance: 3, 
                    out _spawnPosition);
        }
    }
}