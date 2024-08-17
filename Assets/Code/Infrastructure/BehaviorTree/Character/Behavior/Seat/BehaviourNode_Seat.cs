using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Components.Items;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.Character.Sub;
using Code.Infrastructure.DI;
using Code.Infrastructure.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Character.Behavior
{
    public partial class BehaviourNode_Seat : BaseNode_Root
    {
        [Header("Character")] 
        private readonly CharacterAnimator _characterAnimator;
        private readonly CollisionObserver _collisionObserver;

        [Header("Services")] 
        private readonly MicrophoneAnalyzer _microphoneAnalyzer;
        private readonly CharacterCondition _characterCondition;

        [Header("Sub nodes")] 
        private readonly SubNode_ReactionToItems _node_ReactionToItem;
        private readonly SubNode_ReactionToVoice _node_ReactionToVoice;

        public BehaviourNode_Seat()
        {
            var character = Container.Instance.FindEntity<DIVA>();
            //character-------------------------------------------------------------------------------------------------
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();
            //services--------------------------------------------------------------------------------------------------
            _characterCondition = Container.Instance.FindService<CharacterCondition>();
            _microphoneAnalyzer = Container.Instance.FindService<MicrophoneAnalyzer>();
            //nodes-----------------------------------------------------------------------------------------------------
            _node_ReactionToItem = new SubNode_ReactionToItems();
            _node_ReactionToVoice = new SubNode_ReactionToVoice();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                SubscribeToEvents(true);

                _characterAnimator.EnterToMode(CharacterAnimationMode.Seat);

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