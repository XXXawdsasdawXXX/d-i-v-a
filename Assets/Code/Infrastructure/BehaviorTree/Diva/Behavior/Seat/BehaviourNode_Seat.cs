using Code.Data;
using Code.Entities.Common;
using Code.Entities.Diva;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public partial class BehaviourNode_Seat : BaseNode_Root
    {
        [Header("Character")] 
        private readonly DivaAnimator _divaAnimator;
        private readonly CollisionObserver _collisionObserver;

        [Header("Services")] 
        private readonly CharacterCondition _characterCondition;

        [Header("Sub nodes")] 
        private readonly SubNode_ReactionToItems _node_ReactionToItem;
        private readonly SubNode_HideHand _node_HideHand;
        

        public BehaviourNode_Seat()
        {
            DivaEntity character = Container.Instance.FindEntity<DivaEntity>();

            //character-------------------------------------------------------------------------------------------------
            _divaAnimator = character.FindCharacterComponent<DivaAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();

            //services--------------------------------------------------------------------------------------------------
            _characterCondition = Container.Instance.FindService<CharacterCondition>();

            //nodes-----------------------------------------------------------------------------------------------------
            _node_ReactionToItem = new SubNode_ReactionToItems();
            _node_HideHand = new SubNode_HideHand();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                SubscribeToEvents(true);

                _divaAnimator.EnterToMode(EDivaAnimationMode.Seat);

                RunNode(_node_ReactionToItem);

#if DEBUGGING
                Debugging.Log(this, "[run]", Debugging.Type.BehaviorTree);
#endif
            }
            else
            {
#if DEBUGGING
                Debugging.Log(this, $"[run] Return.", Debugging.Type.BehaviorTree);
#endif
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _characterCondition.IsCanSeat();
        }
    }
}