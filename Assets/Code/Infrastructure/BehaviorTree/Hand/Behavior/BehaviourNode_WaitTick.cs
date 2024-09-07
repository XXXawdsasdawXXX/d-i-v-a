using System.Collections;
using Code.Components.Entities.Hands;
using Code.Data.Configs;
using Code.Data.Storages;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Infrastructure.Services.Interactions;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Hand.Behavior
{
    public class BehaviourNode_WaitTick : BaseNode
    {
        [Header("Hand")] //☺
        private readonly HandAnimator _handAnimator;

        [Header("Services")] 
        private readonly InteractionStorage _interactionStorage;
        private readonly TickCounter _tickCounter;
        private readonly CoroutineRunner _coroutineRunner;
        private readonly Interaction_ReturnAfterAbsence _returnAfterAbsence;

        [Header("Static values")] 
        private readonly HandConfig _handConfig;
        private readonly WhiteBoard_Hand _whiteBoard;

        [Header("Dynamic values")] 
        private float _cooldown;
        private Coroutine _waitingCoroutine;

        public BehaviourNode_WaitTick()
        {
            //hand
            var hand = Container.Instance.FindEntity<Components.Entities.Hands.Hand>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();

            //services
            _tickCounter = new TickCounter(isLoop: false);
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _coroutineRunner = Container.Instance.FindService<CoroutineRunner>();
            _returnAfterAbsence = Container.Instance.FindInteractionObserver<Interaction_ReturnAfterAbsence>();

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

                var cooldownTicks = _handConfig.GetVoidTime(_interactionStorage.GetSum());
                _tickCounter.StartWait(cooldownTicks);

                _tickCounter.OnWaitIsOver += OnWaitedTicksEvent;

                Debugging.Instance.Log(this,$"run await {cooldownTicks} ticks", Debugging.Type.Hand);
            }
        }

        protected override bool IsCanRun()
        {
            return _tickCounter.IsExpectedStart;
        }

        private void OnWaitedTicksEvent()
        {
            _tickCounter.OnWaitIsOver -= OnWaitedTicksEvent;
            Debugging.Instance.Log(this,$"дождался тиков", Debugging.Type.Hand);
            _coroutineRunner.StopRoutine(_waitingCoroutine);
            _waitingCoroutine = _coroutineRunner.StartRoutine(WaitReturnAfterAbsence());
        }

        private IEnumerator WaitReturnAfterAbsence()
        {
            yield return new WaitUntil(() => !_returnAfterAbsence.IsAbsence);
            yield return new WaitForSeconds(30);
            Debugging.Instance.Log(this,$" дождался пользователя, return(тру)", Debugging.Type.Hand);
            Return(true);
        }
    }
}