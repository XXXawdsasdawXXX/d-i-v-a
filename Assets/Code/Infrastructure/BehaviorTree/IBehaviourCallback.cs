using Code.Infrastructure.BehaviorTree.BaseNodes;

namespace Code.Infrastructure.BehaviorTree
{
    public interface IBehaviourCallback
    {
        void InvokeCallback(BaseNode node, bool success);
    }
}