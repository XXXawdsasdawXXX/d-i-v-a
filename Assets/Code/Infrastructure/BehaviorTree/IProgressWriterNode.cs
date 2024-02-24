namespace Code.Infrastructure.BehaviorTree
{
    public interface IProgressWriterNode
    {
        void UpdateData(WhiteBoard.Data data);
        
        void LoadData(WhiteBoard.Data data);
    }
}