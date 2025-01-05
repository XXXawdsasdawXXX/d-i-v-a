using Code.Components.Items;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public partial class BehaviourNode_Seat 
    {
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
            if (obj.TryGetComponent(out Item item) && item.IsCanUse())
            {
                Debugging.Instance.Log($"Нода сидения: начинает реакцию на итем ", Debugging.Type.BehaviorTree);
               // _node_ReactionToItem.SetCurrentItem(item);
                RunNode(_node_ReactionToItem);
            }
        }
    }
}