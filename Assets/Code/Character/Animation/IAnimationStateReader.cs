namespace Code.Character
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        
        CharacterAnimatorState State { get; }
    }
}