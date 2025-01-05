namespace Code.Components.Entities
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
    }
}