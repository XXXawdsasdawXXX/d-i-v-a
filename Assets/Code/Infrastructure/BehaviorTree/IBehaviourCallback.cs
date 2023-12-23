namespace Code.Infrastructure.BehaviorTree
{
    public interface IBehaviourCallback
    {
        void Invoke(BehaviourNode node, bool success);
    }
}