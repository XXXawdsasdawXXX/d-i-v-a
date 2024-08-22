using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Components.NewItems;
using Code.Components.Entities.Hands;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand.Behavior
{
    public class BehaviourNode_ShowItem : BaseNode
    {
        [Header("DIVA")] 
        private readonly Transform _divaTransform;

        [Header("Hand")] 
        private readonly Transform _handTransform;
        private readonly HandAnimator _handAnimation;
        private readonly MovementToMouse _handMovementToMouse;
        private readonly ItemHolder _handItemHolder;

        [Header("Services")] 
        private readonly PositionService _positionService;
        private readonly ItemSpawner _itemSpawner;

        [Header("Dynamic data")] 
        private Item _item;
        private ItemData _currentItemData;
        private Vector3 _spawnPosition;


        public BehaviourNode_ShowItem()
        {
            //character-------------------------------------------------------------------------------------------------
            _divaTransform = Container.Instance.FindEntity<DIVA>().transform;
            //hand------------------------------------------------------------------------------------------------------
            var hand = Container.Instance.FindEntity<Components.Entities.Hands.Hand>();
            _handAnimation = hand.FindHandComponent<HandAnimator>();
            _handMovementToMouse = hand.FindCommonComponent<MovementToMouse>();
            _handItemHolder = hand.FindCommonComponent<ItemHolder>();
            _handTransform = hand.transform;
            //services--------------------------------------------------------------------------------------------------
            _itemSpawner = Container.Instance.FindService<ItemSpawner>();
            _positionService = Container.Instance.FindService<PositionService>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _handTransform.position = _spawnPosition;
                _handAnimation.PlayEnterHand(onEndEnter: () =>
                {
                    _item = _itemSpawner.SpawnRandomItem();
                    _handItemHolder.SetItem(_item);
                    _handMovementToMouse.On();
                    SubscribeToEvents(true);
                });
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

        private void HideHand()
        {
            SubscribeToEvents(false);
            _handMovementToMouse.Off();
            _handItemHolder.DropItem();
            _handAnimation.PlayExitHand(() =>
            {
                Return(true);
            });
        }
    }
}