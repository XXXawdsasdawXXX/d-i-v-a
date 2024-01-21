using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Utils;

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
            Debugging.Instance.Log($"Нода смотреть за курсором: запущено ",Debugging.Type.BehaviorTree);
        }
        
        void IBehaviourCallback.InvokeCallback(BehaviourNode node, bool success)
        {
            Return(true);    
        }
    }
}