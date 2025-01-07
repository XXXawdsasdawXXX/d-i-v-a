using Code.Components.Items;
using Code.Utils;
using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.Diva
{
    public partial class BehaviourNode_Stand 
    {
        protected override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _collisionObserver.EnterEvent += _startReactionToObject;
            }
            else
            {
                _collisionObserver.EnterEvent -= _startReactionToObject;
            }
        }

        private void _startReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out Item item) && item.IsCanUse())
            {
                Debugging.Instance.Log(this, $"start reaction to object", Debugging.Type.BehaviorTree);
            
                _node_reactionToItem.SetCurrentItem(item);
                
                RunNode(_node_reactionToItem);
            }
        }
    }
}