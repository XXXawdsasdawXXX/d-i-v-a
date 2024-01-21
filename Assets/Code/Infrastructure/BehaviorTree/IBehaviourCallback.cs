namespace Code.Infrastructure.BehaviorTree
{
    public interface IBehaviourCallback
    {
        void InvokeCallback(BehaviourNode node, bool success);
    }
}