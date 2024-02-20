namespace Code.Infrastructure.BehaviorTree
{
    public class BaseNode_Root : BaseNode,IBehaviourCallback
    {
        private BaseNode _node_Current;
        protected override void Run() { }
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

        protected virtual void SubscribeToEvents(bool b)
        {
            
        }
        
        public virtual void InvokeCallback(BaseNode node, bool success)
        {
            
        }
    }
}