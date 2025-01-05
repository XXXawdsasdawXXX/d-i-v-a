using Code.Components.Entities;
using Code.Data.Enums;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.BehaviorTree.Hand
{
    public class BehaviourNode_WaitCharacterWakeUp : BaseNode
    {
        private readonly DivaAnimationAnalytic _animationAnalytic;

        public BehaviourNode_WaitCharacterWakeUp()
        {
            Components.Entities.Diva diva = Container.Instance.FindEntity<Components.Entities.Diva>();
            _animationAnalytic = diva.FindCharacterComponent<DivaAnimationAnalytic>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                _subscribeToEvents(true);
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _animationAnalytic.GetAnimationMode() is CharacterAnimationMode.Sleep;
        }

        private void _subscribeToEvents(bool flag)
        {
            if (flag)
            {
                _animationAnalytic.OnEnteredMode += _onEnteredMode;
            }
            else
            {
                _animationAnalytic.OnEnteredMode -= _onEnteredMode;
            }
        }

        private void _onEnteredMode(CharacterAnimationMode currentMode)
        {
            if (currentMode is not CharacterAnimationMode.Sleep)
            {
                _subscribeToEvents(false);
              
                Return(true);
            }
        }
    }
}