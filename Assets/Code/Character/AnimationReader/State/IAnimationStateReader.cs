namespace Code.Character.AnimationReader.State
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        
        CharacterAnimationState State { get; }
    }
}