using Code.Components.Character;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.CharacterStateMachine.States
{
    public class SeatState : IState
    {
        private readonly Character _character;

        public SeatState(Container container)
        {
            _character = container.GetCharacter();
        }

        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}