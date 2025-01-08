using Code.Entities.Items;
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
                _collisionObserver.EnterEvent += _startReactionToObject;
            }
            else
            {
                _collisionObserver.EnterEvent -= _startReactionToObject;
            }
        }

        private void _startReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out ItemEntity item) && item.IsCanUse())
            {
#if DEBUGGING
                Debugging.Log(this, $"[_startReactionToObject]", Debugging.Type.BehaviorTree);
#endif
                RunNode(_node_ReactionToItem);
            }
        }
    }
}