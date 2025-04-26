using Code.Entities.Items;
using Code.Utils;
using UnityEngine;

namespace Code.BehaviorTree.Diva
{
    public partial class BehaviourNode_Stand
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
            RunNode(_node_HideHand);
        }

        private void _startReactionToObject(GameObject obj)
        {
            if (obj.TryGetComponent(out ItemEntity item) && item.IsCanUse())
            {
                Log.Info(this, $"[_startReactionToObject]", Log.Type.BehaviorTree);
                _node_reactionToItem.SetCurrentItem(item);
                
                RunNode(_node_reactionToItem);
            }
        }
    }
}