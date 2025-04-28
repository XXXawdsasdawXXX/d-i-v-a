using Code.Game.Entities.Common;
using Code.Game.Entities.Diva;
using Code.Game.Entities.Hand;
using Code.Infrastructure.ServiceLocator;
using Code.Utils;
using UnityEngine;

namespace Code.Game.BehaviorTree.Diva
{
    public partial class BehaviourNode_Seat : BaseNode_Root
    {
        [Header("diva")] 
        private readonly DivaAnimator _divaAnimator;
        private readonly CollisionObserver _divaCollision;
        private readonly DivaCondition _divaCondition;
        
        [Header("hand")] 
        private readonly HandBehaviorEvents _handEvents;

        [Header("Sub nodes")] 
        private readonly SubNode_ReactionToItems _node_reactionToItem;
        private readonly SubNode_HideHand _node_hideHand;

        public BehaviourNode_Seat()
        {
            //diva------------------------------------------------------------------------------------------------------
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
            _divaAnimator = diva.FindCharacterComponent<DivaAnimator>();
            _divaCollision = diva.FindCommonComponent<CollisionObserver>();
            _divaCondition = Container.Instance.GetService<DivaCondition>();

            //hand------------------------------------------------------------------------------------------------------
            HandEntity hand = Container.Instance.FindEntity<HandEntity>();
            _handEvents = hand.FindHandComponent<HandBehaviorEvents>();
            
            //nodes-----------------------------------------------------------------------------------------------------
            _node_reactionToItem = new SubNode_ReactionToItems();
            _node_hideHand = new SubNode_HideHand();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                SubscribeToEvents(true);

                _divaAnimator.EnterToMode(EDivaAnimationMode.Seat);
                Log.Info(this, "[run]", Log.Type.BehaviorTree);
            }
            else
            {
                Return(false);
                Log.Info(this, $"[run] Return.", Log.Type.BehaviorTree);
            }
        }

        protected override bool IsCanRun()
        {
            return _divaCondition.IsCanSeat();
        }
    }
}