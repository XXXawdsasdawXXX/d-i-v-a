using Code.Components.Apples;
using Code.Components.Hands;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Infrastructure.BehaviorTree.CustomNodes.Character.Sub;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Hand.Behavior
{
    public class BehaviourNode_ShowApple : BaseNode
    {
        [Header("Entities")] private readonly Apple _apple;

        [Header("Hand")] //☺
        private readonly ColliderButton _handButton;

        private readonly HandMovement _handMovement;
        private readonly HandAnimator _handAnimator;
        private readonly ItemHolder _itemHolder;

        [Header("Services")] private TickCounter _tickCounter;

        [Header("Static values")] private readonly HandConfig _handConfig;
        private readonly InteractionStorage _interactionsStorage;
        private readonly WhiteBoard_Hand _whiteBoard;
        private readonly AppleConfig _appleConfig;

        [Header("Dynamic values")] private bool _isReady = true;


        public BehaviourNode_ShowApple()
        {
            //enitities
            _apple = Container.Instance.FindItem<Apple>();
            //hand
            var hand = Container.Instance.FindEntity<Components.Entities.Hand>();
            _handButton = hand.FindCommonComponent<ColliderButton>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();
            _handMovement = hand.FindHandComponent<HandMovement>();
            _itemHolder = hand.FindCommonComponent<ItemHolder>();

            //static data
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _interactionsStorage = Container.Instance.FindStorage<InteractionStorage>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
            _appleConfig = Container.Instance.FindConfig<AppleConfig>();

            //services 
            _tickCounter = new TickCounter(isLoop: false);
        }

        ~BehaviourNode_ShowApple()
        {
        }


        protected override void Run()
        {
            if (!_isReady)
            {
                Return(false);
                return;
            }

            var dropChance = _handConfig.GetAppleDropChance(_interactionsStorage.GetSum());
            var random = Random.Range(0, 101);
            if (random <= dropChance)
            {
                var cooldownTicks = _appleConfig.SpawnCooldownTick.GetRandomValue();
                _tickCounter.StartWait(cooldownTicks, () => _isReady = true);
                _isReady = false;
                Debugging.Instance.Log($"[showapple_run] drop! new cooldown {cooldownTicks}", Debugging.Type.Hand);
                ShowWhitApple();
                return;
            }

            Debugging.Instance.Log(
                $"[showapple_run] not any drop! {random} > {dropChance}. interaction count = {_interactionsStorage.GetSum()}",
                Debugging.Type.Hand);

            _isReady = true;
            Return(false);
        }

        private void ShowWhitApple()
        {
            _itemHolder.SetItem(_apple);
            _apple.Grow();
            _handAnimator.PlayEnter();
            _handMovement.On();
            SubscribeToEvents(true);
            Debugging.Instance.Log($"[showapple_run] Start", Debugging.Type.Hand);
        }


        private void HandButtonOnUpEvent(Vector2 arg1, float arg2)
        {
            _itemHolder.DropItem();
            _apple.Fall();
            _handAnimator.PlayExit();
            _handMovement.Off();
            SubscribeToEvents(false);
            Debugging.Instance.Log($"[showapple_run] Stop", Debugging.Type.Hand);
            Return(true);
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _handButton.UpEvent += HandButtonOnUpEvent;
            }
            else
            {
                _handButton.UpEvent -= HandButtonOnUpEvent;
            }
        }
    }
}