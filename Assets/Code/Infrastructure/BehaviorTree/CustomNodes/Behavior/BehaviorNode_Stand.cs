using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Items;
using Code.Components.Objects;
using Code.Data.Configs;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Stand : BaseNode_Root, IBehaviourCallback
    {
        [Header("Character")] 
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CollisionObserver _collisionObserver;

        [Header("Node")] 
        private BaseNode _node_Current;
        private readonly BaseNode_RandomSequence _node_randomSequence;
        private readonly SubNode_ReactionToItems _node_ReactionToItem;

        public BehaviorNode_Stand()
        {
            //character-------------------------------------------------------------------------------------------------
            var character = Container.Instance.FindEntity<DIVA>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();
            //node------------------------------------------------------------------------------------------------------
            _node_randomSequence = new BaseNode_RandomSequence(new BaseNode[]
            {
                new SubNode_WaitForTicks(Container.Instance.FindConfig<TimeConfig>().Duration.Stand),
                new SubNode_LookToMouse()
            });
            _node_ReactionToItem = new SubNode_ReactionToItems();
        }

        protected override void Run()
        {
            if (IsCanStand())
            {
                Debugging.Instance.Log($"Нода стояния: выбрано", Debugging.Type.BehaviorTree);
                SubscribeToEvents(true);
                _characterAnimator.EnterToMode(CharacterAnimationMode.Stand);
                RunNode(_node_randomSequence);
            }
            else
            {
                Debugging.Instance.Log($"Нода стояния: отказ. текущий минимальный стрейт {_statesAnalytic.CurrentLowerLiveStateKey}", Debugging.Type.BehaviorTree);
                Return(false);
            }
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Debugging.Instance.Log($"Нода стояния: колбэк. продолжение работы ноды = {_statesAnalytic.CurrentLowerLiveStateKey == LiveStateKey.None && success}", Debugging.Type.BehaviorTree);
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
                _node_ReactionToItem.SetCurrentItem(item);
                RunNode(_node_ReactionToItem);
            }
        }

        #endregion

        #region Conditions

        private bool IsCanStand()
        {
            return true;
        }

        #endregion
    }
}