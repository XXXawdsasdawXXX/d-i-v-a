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
    public partial class BehaviourNode_Seat 
    {
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
                _microphoneAnalyzer.MaxDecibelRecordedEvent -= OnMaxDecibelRecordedEvent;
            }
        }

        private void OnMaxDecibelRecordedEvent()
        {
            RunNode(_node_ReactionToVoice);
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
    }
}