using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Components.Items;
using Code.Components.Objects;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.BehaviorTree.CustomNodes.Sub;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Seat : BaseNode_Root
    {
        [Header("Character")]
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;
        private readonly CollisionObserver _collisionObserver;
        
        [Header("Services")]
        private readonly MicrophoneAnalyzer _microphoneAnalyzer;
        
        [Header("Sub nodes")] 
        private readonly SubNode_ReactionToItems _node_ReactionToItem;
        private readonly SubNode_ReactionToVoice _node_ReactionToVoice;

        public BehaviorNode_Seat()
        {
            var character = Container.Instance.FindEntity<DIVA>();
            //character-------------------------------------------------------------------------------------------------
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();
            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _collisionObserver = character.FindCommonComponent<CollisionObserver>();
            //services--------------------------------------------------------------------------------------------------
            _microphoneAnalyzer = Container.Instance.FindService<MicrophoneAnalyzer>();
            //nodes-----------------------------------------------------------------------------------------------------
            _node_ReactionToItem = new SubNode_ReactionToItems();
            _node_ReactionToVoice = new SubNode_ReactionToVoice();
        }

        protected override void Run()
        {
            TrySeat();
        }
        
        private void TrySeat()
        {
            if (IsCanSeat())
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

        /*
        public override void InvokeCallback(BaseNode node, bool success)
        {
            TrySeat();
            base.InvokeCallback(node, success);
        }
        */

        
        #region Events

        protected override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _collisionObserver.EnterEvent += StartReactionToObject;
                _microphoneAnalyzer.MaxDecibelRecordedEvent += OnMaxDecibelRecordedEvent;
            }
            else
            {
                _collisionObserver.EnterEvent -= StartReactionToObject;
            }
        }

        private void OnMaxDecibelRecordedEvent()
        {
            if (_node_ReactionToVoice.IsReady())
            {
                RunNode(_node_ReactionToVoice);
            }
        }

        private void StartReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Item item))
            {
                Debugging.Instance.Log($"Нода сидения: начинает реакцию на итем ", Debugging.Type.BehaviorTree);
                _node_ReactionToItem.SetCurrentItem(item);
                RunNode(_node_ReactionToItem);
            }
        }

        #endregion

        
        #region Condition

        private bool IsCanSeat()
        {
            return _statesAnalytic.TryGetLowerSate(out var key, out var statePercent)
                   && statePercent < 0.4f
                   && key is LiveStateKey.Trust or LiveStateKey.Hunger;
        }

        #endregion
    }
}