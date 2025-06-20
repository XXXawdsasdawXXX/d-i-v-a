﻿namespace Code.Game.BehaviorTree
{
    public abstract class BaseNode_Root : BaseNode, IBehaviourCallback
    {
        private BaseNode _node_Current;

        protected override void OnBreak()
        {
            _node_Current?.Break();
            _node_Current = null;
            SubscribeToEvents(false);
            base.OnBreak();
        }

        protected override void OnReturn(bool success)
        {
            _node_Current?.Break();
            _node_Current = null;
            SubscribeToEvents(false);
            base.OnReturn(success);
        }

        protected void RunNode(BaseNode node)
        {
            _node_Current?.Break();
            _node_Current = node;
            _node_Current.Run(this);
        }
        
        protected void ChildBreak()
        {
            _node_Current?.Break();
        }

        protected bool IsEqualsChild(BaseNode node)
        {
            return _node_Current != null && _node_Current.Equals(node);
        }
        
        protected virtual void SubscribeToEvents(bool flag)
        {
        }

        public virtual void InvokeCallback(BaseNode node, bool success)
        {
        }
    }
}