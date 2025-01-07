using Code.Components.Common;
using Code.Components.Entities;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public partial class BehaviourNode_Stand : BaseNode_Root, IBehaviourCallback
    {
        [Header("Character")] 
        private readonly DivaAnimator _divaAnimator;
        private readonly DivaLiveStatesAnalytic _statesAnalytic;
        private readonly CollisionObserver _collisionObserver;

        [Header("Services")]
        private readonly CharacterCondition _characterCondition;

        [Header("Sub Node")] 
        private readonly BaseNode_RandomSequence _node_randomSequence;
        private readonly SubNode_ReactionToItems _node_reactionToItem;
        private BaseNode _currentSubNode;


        public BehaviourNode_Stand()
        {
            //character-------------------------------------------------------------------------------------------------
            Components.Entities.Diva character = Container.Instance.FindEntity<Components.Entities.Diva>();
            _statesAnalytic = character.FindCharacterComponent<DivaLiveStatesAnalytic>();
            _divaAnimator = character.FindCharacterComponent<DivaAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();

            //services--------------------------------------------------------------------------------------------------
            _characterCondition = Container.Instance.FindService<CharacterCondition>();

            //node------------------------------------------------------------------------------------------------------
            _node_randomSequence = new BaseNode_RandomSequence(new BaseNode[]
            {
                new SubNode_WaitForTicks(Container.Instance.FindConfig<TimeConfig>().Duration.Stand),
                new SubNode_LookToMouse()
            });
           
            _node_reactionToItem = new SubNode_ReactionToItems();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _divaAnimator.EnterToMode(EDivaAnimationMode.Stand);

                SubscribeToEvents(true);

                RunNode(_node_randomSequence);

                Debugging.Log(this, $"[run]", Debugging.Type.BehaviorTree);
            }
            else
            {
                Debugging.Log(this, $"[run] Return -> has low state {_statesAnalytic.CurrentLowerLiveStateKey}",
                    Debugging.Type.BehaviorTree);
                
                Return(false);
            }
        }

        protected override bool IsCanRun()
        {
            return _characterCondition.IsCanStand();
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Log(this, $"[InvokeCallback] Repeat = {_statesAnalytic.CurrentLowerLiveStateKey == ELiveStateKey.None && success}",
                Debugging.Type.BehaviorTree);
            
            if (_statesAnalytic.CurrentLowerLiveStateKey == ELiveStateKey.None && success)
            {
                RunNode(_node_randomSequence);
            }
        }
    }
}