using Code.Components.Hands;
using Code.Data.Configs;
using Code.Data.Storages;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
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
        private readonly TickCounter _tickCounter;

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
            _tickCounter = new TickCounter(isLoop: false);
            
            //static value
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
        }
        

        protected override void Run()
        {
            if (IsCanRun())
            {
                _whiteBoard.SetData(WhiteBoard_Hand.Type.IsHidden, true);
                _handAnimator.PlayExitHand();
                _handMovement.Off();
                
                var cooldownTicks = _handConfig.GetVoidTime(_interactionStorage.GetSum());
                _tickCounter.StartWait(cooldownTicks);
                _tickCounter.WaitedEvent += OnWaitedTicksEvent;
                
                Debugging.Instance.Log($"[enter the void!] run await {cooldownTicks} ticks", Debugging.Type.Hand);
            }
        }

        protected override bool IsCanRun()
        {
            return _tickCounter.IsExpectedStart;
        }

        private void OnWaitedTicksEvent()
        {
            _tickCounter.WaitedEvent -= OnWaitedTicksEvent;
            Debugging.Instance.Log($"[enter the void!] дождался тиков, return(тру)", Debugging.Type.Hand);
            Return(true);
        }
    }
}