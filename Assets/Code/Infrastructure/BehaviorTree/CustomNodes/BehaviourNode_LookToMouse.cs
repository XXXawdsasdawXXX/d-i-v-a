using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviourNode_LookToMouse : BehaviourNode, IBehaviourCallback
    {
        private BehaviourNode_WaitForSeconds _waitFor;

        public BehaviourNode_LookToMouse()
        {
            _waitFor = new BehaviourNode_WaitForSeconds(new RangedFloat()
            {
                MinValue = 60 * 1,
                MaxValue = 60 * 5
            });
        }
        
        protected override void Run()
        {
            _waitFor.Run(this);
        }
        
        void IBehaviourCallback.Invoke(BehaviourNode node, bool success)
        {
            Return(true);    
        }
    }
}