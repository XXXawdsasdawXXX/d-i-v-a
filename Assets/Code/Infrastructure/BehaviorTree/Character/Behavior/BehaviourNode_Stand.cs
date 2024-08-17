using Code.Components.Common;
using Code.Components.Entities.Characters;
using Code.Components.Items;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.Character.Sub;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Character.Behavior
{
    public class BehaviourNode_Stand : BaseNode_Root, IBehaviourCallback
    {
        [Header("Character")] private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CollisionObserver _collisionObserver;

        [Header("Services")] private readonly CharacterCondition _characterCondition;

        [Header("Node")] private readonly BaseNode_RandomSequence _node_randomSequence;
        private readonly SubNode_ReactionToItems _node_reactionToItem;
        private BaseNode _node_Current;

        public BehaviourNode_Stand()
        {
            //character-------------------------------------------------------------------------------------------------
            var character = Container.Instance.FindEntity<DIVA>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
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
                _characterAnimator.EnterToMode(CharacterAnimationMode.Stand);

                SubscribeToEvents(true);

                RunNode(_node_randomSequence);

                Debugging.Instance.Log($"Нода стояния: выбрано", Debugging.Type.BehaviorTree);
            }
            else
            {
                Debugging.Instance.Log(
                    $"Нода стояния: отказ. текущий минимальный стрейт {_statesAnalytic.CurrentLowerLiveStateKey}",
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
            Debugging.Instance.Log(
                $"Нода стояния: колбэк. продолжение работы ноды = {_statesAnalytic.CurrentLowerLiveStateKey == LiveStateKey.None && success}",
                Debugging.Type.BehaviorTree);
            if (_statesAnalytic.CurrentLowerLiveStateKey == LiveStateKey.None && success)
            {
                RunNode(_node_randomSequence);
            }
        }

        #region Events

        protected override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _collisionObserver.EnterEvent += StartReactionToObject;
            }
            else
            {
                _collisionObserver.EnterEvent -= StartReactionToObject;
            }
        }

        private void StartReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Item item))
            {
                Debugging.Instance.Log($"Нода стояния: начинает реакцию на итем ", Debugging.Type.BehaviorTree);
                _node_reactionToItem.SetCurrentItem(item);
                RunNode(_node_reactionToItem);
            }
        }

        #endregion
    }
}