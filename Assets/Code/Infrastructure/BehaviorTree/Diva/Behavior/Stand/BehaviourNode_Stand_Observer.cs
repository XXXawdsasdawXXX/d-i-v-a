﻿using Code.Entities.Items;
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
            if (obj.TryGetComponent(out ItemEntity item) && item.IsCanUse())
            {
#if DEBUGGING
                Debugging.Log(this, $"[_start reaction to object] {item.Data.Type}", Debugging.Type.BehaviorTree);
#endif
                _node_reactionToItem.SetCurrentItem(item);

                RunNode(_node_reactionToItem);
            }
        }
    }
}