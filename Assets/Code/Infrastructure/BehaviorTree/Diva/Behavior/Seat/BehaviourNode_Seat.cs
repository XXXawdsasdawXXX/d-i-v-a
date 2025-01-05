using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Enums;
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
        private readonly SubNode_ReactionToVoice _node_ReactionToVoice;

        public BehaviourNode_Seat()
        {
            Components.Entities.Diva character = Container.Instance.FindEntity<Components.Entities.Diva>();
          
            //character-------------------------------------------------------------------------------------------------
            _divaAnimator = character.FindCharacterComponent<DivaAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();
            
            //services--------------------------------------------------------------------------------------------------
            _characterCondition = Container.Instance.FindService<CharacterCondition>();
            
            //nodes-----------------------------------------------------------------------------------------------------
            _node_ReactionToItem = new SubNode_ReactionToItems();
            _node_ReactionToVoice = new SubNode_ReactionToVoice();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                SubscribeToEvents(true);

                _divaAnimator.EnterToMode(EDivaAnimationMode.Seat);

                RunNode(_node_ReactionToItem);

                Debugging.Instance.Log($"Нода сидения: выбрано", Debugging.Type.BehaviorTree);
            }
            else
            {
                Debugging.Instance.Log($"Нода сидения: отказ ", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _characterCondition.IsCanSeat();
        }
    }
}