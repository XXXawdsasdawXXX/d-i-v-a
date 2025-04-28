using System.Collections;
using Code.Data;
using Code.Game.Entities.Hand;
using Code.Game.Services;
using Code.Game.Services.Interactions;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using UnityEngine;

namespace Code.Game.BehaviorTree.Hand
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
            HandEntity hand = Container.Instance.FindEntity<HandEntity>();
            _handAnimator = hand.FindHandComponent<HandAnimator>();

            //services
            _tickCounter = new TickCounter(isLoop: false);
            _interactionStorage = Container.Instance.FindStorage<InteractionStorage>();
            _coroutineRunner = Container.Instance.GetService<CoroutineRunner>();
            _returnAfterAbsence = Container.Instance.FindInteractionObserver<Interaction_ReturnAfterAbsence>();

            //static value
            _handConfig = Container.Instance.GetConfig<HandConfig>();
            _whiteBoard = Container.Instance.FindStorage<WhiteBoard_Hand>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _whiteBoard.SetData(WhiteBoard_Hand.Type.IsHidden, true);

                _handAnimator.PlayExitHand();

                int cooldownTicks = _handConfig.GetVoidTime(_interactionStorage.GetSum());

                _tickCounter.StartWait(cooldownTicks);

                _tickCounter.OnWaitIsOver += _onWaitedTicksEvent;

                Log.Info(this, $"[run] Await {cooldownTicks} ticks.", Log.Type.Hand);
            }
        }

        protected override bool IsCanRun()
        {
            return _tickCounter.IsExpectedStart;
        }

        private void _onWaitedTicksEvent()
        {
            _tickCounter.OnWaitIsOver -= _onWaitedTicksEvent;

            Log.Info(this, "[_on waited ticks] Start routine.", Log.Type.Hand);

            _coroutineRunner.StopRoutine(_waitingCoroutine);

            _waitingCoroutine = _coroutineRunner.StartRoutine(_waitReturnAfterAbsence());
        }

        private IEnumerator _waitReturnAfterAbsence()
        {
            yield return new WaitUntil(() => !_returnAfterAbsence.IsAbsence);

            yield return new WaitForSeconds(Random.Range(0, 5));

            Log.Info(this, "[_on waited user] End routine.", Log.Type.Hand);

            Return(true);
        }
    }
}