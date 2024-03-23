using Code.Components.Hands;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes.Hand.Behavior
{
    public class BehaviourNode_EnterTheVoid : BaseNode
    {
        [Header("Hand")] //☺
        private readonly HandAnimator _handAnimator;
        private readonly HandMovement _handMovement;

        [Header("Services")] 
        private readonly InteractionStorage _interactionStorage;
        private readonly TickCounter _tickCounter = new();

        [Header("Static values")] 
        private readonly HandConfig _handConfig;
        private readonly WhiteBoard_Hand _whiteBoard;
        [Header("Dynamic values")] 
        private float _cooldown;



        public BehaviourNode_EnterTheVoid()
        {
            //hand
            var hand = Container.Instance.FindEntity<Components.Entities.Hand>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();
            _handMovement = hand.FindHandComponent<HandMovement>();
         

            //services
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();

            //static value
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
        }

        ~BehaviourNode_EnterTheVoid()
        {
        }

        protected override void Run()
        {
            var cooldownTicks = _handConfig.GetVoidTime(_interactionStorage.GetSum());
            Debugging.Instance.Log($"[showapple_run] enter the void! await {cooldownTicks} ticks",Debugging.Type.Hand);
            _tickCounter.StartWait(cooldownTicks);
            if (_whiteBoard.TryGetData<bool>(WhiteBoard_Hand.Type.IsHidden, out bool isHidden) && !isHidden)
            {
                _whiteBoard.SetData(WhiteBoard_Hand.Type.IsHidden, true);
                _handAnimator.PlayExit();
                _handMovement.Off();
            }
            
            _tickCounter.WaitedEvent += TickCounterOnWaitedEvent;
        }

        private void TickCounterOnWaitedEvent()
        {
            _tickCounter.WaitedEvent -= TickCounterOnWaitedEvent;
            Return(true);
        }
    }
}