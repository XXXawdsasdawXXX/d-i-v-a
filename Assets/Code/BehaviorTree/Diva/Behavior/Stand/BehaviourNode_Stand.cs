using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Entities.Hand;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.BehaviorTree.Diva
{
    public partial class BehaviourNode_Stand : BaseNode_Root, IBehaviourCallback
    {
        [Header("Diva")] 
        private readonly DivaAnimator _divaAnimator;
        private readonly DivaLiveStatesAnalytic _divaStatesAnalytic;
        private readonly CollisionObserver _divaCollision;
        private readonly DivaCondition _divaCondition;

        [Header("Hand")]
        private readonly HandBehaviorEvents _handEvents;

        [Header("Sub Node")] 
        private readonly BaseNode_RandomSequence _node_randomSequence;
        private readonly SubNode_ReactionToItems _node_reactionToItem;
        private readonly SubNode_HideHand _node_HideHand;


        public BehaviourNode_Stand()
        {
            //character-------------------------------------------------------------------------------------------------
            DivaEntity character = Container.Instance.FindEntity<DivaEntity>();
            _divaStatesAnalytic = character.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _divaAnimator = character.FindCharacterComponent<DivaAnimator>();
            _divaCollision = character.FindCommonComponent<CollisionObserver>();

            //services--------------------------------------------------------------------------------------------------
            _divaCondition = Container.Instance.GetService<DivaCondition>();
            
            //services--------------------------------------------------------------------------------------------------
            HandEntity hand = Container.Instance.FindEntity<HandEntity>();
            _handEvents = hand.FindHandComponent<HandBehaviorEvents>();
            
            //node------------------------------------------------------------------------------------------------------
            _node_randomSequence = new BaseNode_RandomSequence(new BaseNode[]
            {
                new SubNode_WaitForTicks(Container.Instance.FindConfig<TimeConfig>().Duration.Stand),
                new SubNode_LookToMouse()
            });
            _node_reactionToItem = new SubNode_ReactionToItems();
            _node_HideHand = new SubNode_HideHand();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _divaAnimator.EnterToMode(EDivaAnimationMode.Stand);

                SubscribeToEvents(true);

                RunNode(_node_randomSequence);

#if DEBUGGING
                Debugging.Log(this, $"[run]", Debugging.Type.BehaviorTree);
#endif
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, $"[run] Return -> has low state {_divaStatesAnalytic.CurrentLowerLiveStateKey}.",
                    Debugging.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _divaCondition.IsCanStand();
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
#if DEBUGGING
            Debugging.Log(this,
                $"[InvokeCallback] Repeat = {_divaStatesAnalytic.CurrentLowerLiveStateKey == ELiveStateKey.None && success}.",
                Debugging.Type.BehaviorTree);
#endif
            
            if (_divaStatesAnalytic.CurrentLowerLiveStateKey == ELiveStateKey.None && success)
            {
                RunNode(_node_randomSequence);
            }
        }
    }
}