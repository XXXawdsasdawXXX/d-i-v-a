using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Entities.Hand;
using Code.Entities.Items;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand
{
    public class BehaviourNode_ShowItem : BaseNode
    {
        [Header("d i v a")] 
        private readonly Transform _divaTransform;
        private readonly DivaAnimationAnalytic _divaAnimationAnalytic;

        [Header("Hand")] 
        private readonly HandEntity _hand;
        private readonly HandAnimator _handAnimation;
        private readonly MovementToMouse _movementToMouse;
        private readonly ItemHolder _itemHolder;
        private readonly HandBehaviorEvents _handEvents;

        [Header("Item")] 
        private readonly ItemSpawner _itemSpawner;
        private ItemEntity _item;

        [Header("Services")] 
        private readonly PositionService _positionService;

        [Header("Dynamic data")] 
        private ItemData _currentItemData;
        private Vector3 _spawnPosition;

        public BehaviourNode_ShowItem()
        {
            //diva
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>(); 
            _divaTransform = diva.transform;
            _divaAnimationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();

            //hand
            _hand = Container.Instance.FindEntity<HandEntity>();
            _handAnimation = _hand.FindHandComponent<HandAnimator>();
            _movementToMouse = _hand.FindCommonComponent<MovementToMouse>();
            _itemHolder = _hand.FindCommonComponent<ItemHolder>();
            _handEvents = _hand.FindHandComponent<HandBehaviorEvents>();
            
            //item
            _itemSpawner = Container.Instance.FindService<ItemSpawner>();
            
            //services
            _positionService = Container.Instance.FindService<PositionService>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                
                _handEvents.InvokeWillAppear();
                
                _hand.transform.position = _spawnPosition;
                
                _subscribeToDivaEvents(true);
                
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _positionService
                .TryGetRandomDistantPosition(targetPosition: _divaTransform.position, minDistance: 3, 
                    out _spawnPosition);
        }

        private void _subscribeToDivaEvents(bool flag)
        {
            if (flag)
            {
                _divaAnimationAnalytic.OnSwitchState += _showHand;
            }
            else
            {
                _divaAnimationAnalytic.OnSwitchState -= _showHand;
            }
        }
        
        private void _subscribeToItemEvents(bool flag)
        {
            if (flag)
            {
                _item.OnDestroyed += _hideHand;
                _item.OnPressed += _hideHand;
            }
            else
            {
                _item.OnDestroyed -= _hideHand;
                _item.OnPressed -= _hideHand;
            }
        }

        private void _showHand(EDivaAnimationState state)
        {
            _subscribeToDivaEvents(false);
            
            if (state == EDivaAnimationState.HandIdle)
            {
                _handAnimation.PlayEnterHand(onEndEnter: () =>
                {
                    _item = _itemSpawner.SpawnRandomItem(anchor: _hand.transform);
                   
                    _itemHolder.SetItem(_item);
                    
                    _movementToMouse.Active();
                });
            }
        }

        private void _hideHand()
        {
            _subscribeToItemEvents(false);
            
            _movementToMouse.Disable();
            
            _itemHolder.DropItem();
            
            _handAnimation.PlayExitHand(() =>
            {
                Return(true);
            });
        }
    }
}