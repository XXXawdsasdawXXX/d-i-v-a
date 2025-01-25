using Code.Entities.Items;
using Code.Utils;
using UnityEngine;

namespace Code.BehaviorTree.Diva
{
    public partial class BehaviourNode_Seat
    {
        protected override void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _divaCollision.EnterEvent += _startReactionToObject;
                _handEvents.OnWillAppear += _onHandWillAppear;
                _handEvents.OnHidden += _onHandHidden;
            }
            else
            {
                _divaCollision.EnterEvent -= _startReactionToObject;
                _handEvents.OnWillAppear -= _onHandWillAppear;
                _handEvents.OnHidden -= _onHandHidden;
            }
        }

        private void _onHandHidden()
        {
            ChildBreak();
        }

        private void _onHandWillAppear()
        {
            RunNode(_node_hideHand);
        }

        private void _startReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out ItemEntity item) && item.IsCanUse())
            {
#if DEBUGGING
                Debugging.Log(this, $"[_startReactionToObject]", Debugging.Type.BehaviorTree);
#endif
                _node_reactionToItem.SetCurrentItem(item);
                
                RunNode(_node_reactionToItem);
            }
        }
    }
}