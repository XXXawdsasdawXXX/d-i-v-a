namespace Code.BehaviorTree
{
    public interface IBehaviourCallback
    {
       void InvokeCallback(BaseNode node, bool success);
    }
}