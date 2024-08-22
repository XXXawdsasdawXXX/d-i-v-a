using Code.Components.Entities.Hands;
using Code.Data.Configs;
using Code.Data.Storages;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand.Behavior
{
    public class BehaviourNode_RandomInteractionTickWaiting : BaseNode
    {
        [Header("Hand")] //☺
        private readonly HandAnimator _handAnimator;

        [Header("Services")] 
        private readonly InteractionStorage _interactionStorage;
        private readonly TickCounter _tickCounter;

        [Header("Static values")] 
        private readonly HandConfig _handConfig;
        private readonly WhiteBoard_Hand _whiteBoard;

        [Header("Dynamic values")] 
        private float _cooldown;


        public BehaviourNode_RandomInteractionTickWaiting()
        {
            //hand------------------------------------------------------------------------------------------------------
            var hand = Container.Instance.FindEntity<Components.Entities.Hands.Hand>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();

            //services--------------------------------------------------------------------------------------------------
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _tickCounter = new TickCounter(isLoop: false);

            //static value----------------------------------------------------------------------------------------------
            _handConfig = Container.Instance.FindConfig<HandConfig>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
        }


        protected override void Run()
        {
            if (IsCanRun())
            {
                _whiteBoard.SetData(WhiteBoard_Hand.Type.IsHidden, true);
                _handAnimator.PlayExitHand();

                var cooldownTicks = _handConfig.GetVoidTime(_interactionStorage.GetSum());
                Debugging.Instance.Log(this,$"run await {cooldownTicks} ticks", Debugging.Type.Hand);
                
                if (cooldownTicks == 0)
                {
                    Return(true);
                    return;
                }

                _tickCounter.StartWait(cooldownTicks);
                _tickCounter.OnWaitIsOver += OnWaitedTicksEvent;

                return;
            }
            
            Return(false);
        }

        protected override bool IsCanRun()
        {
            var random = Random.Range(0, 101);
            Debugging.Instance.Log(this,$"random = {random}", Debugging.Type.Hand);
            return random >= 100 - _handConfig.ChanceOfAppearance && _tickCounter.IsExpectedStart;
        }

        protected override void OnBreak()
        {
            _tickCounter.StopWait();
            base.OnBreak();
        }

        private void OnWaitedTicksEvent()
        {
            _tickCounter.OnWaitIsOver -= OnWaitedTicksEvent;
            Debugging.Instance.Log(this,$"дождался тиков, return(тру)", Debugging.Type.Hand);
            Return(true);
        }
    }
}