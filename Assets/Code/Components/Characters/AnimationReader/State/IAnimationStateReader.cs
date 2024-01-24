using Code.Data.Enums;

namespace Code.Components.Characters.AnimationReader.State
{
    public interface IAnimationStateReader
    {
        void EnteredState(int stateHash);
        void ExitedState(int stateHash);
        
        CharacterAnimationState State { get; }
    }
}