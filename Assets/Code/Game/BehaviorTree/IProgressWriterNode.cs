namespace Code.Game.BehaviorTree
{
    public interface IProgressWriterNode
    {
        void UpdateData(BehaviourTreeLoader.Data data);

        void LoadData(BehaviourTreeLoader.Data data);
    }
}