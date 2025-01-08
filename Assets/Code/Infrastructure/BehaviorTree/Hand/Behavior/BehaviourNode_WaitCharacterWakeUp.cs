using Code.Data;
using Code.Entities.Diva;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.BehaviorTree.Hand
{
    public class BehaviourNode_WaitCharacterWakeUp : BaseNode
    {
        private readonly DivaAnimationAnalytic _animationAnalytic;

        public BehaviourNode_WaitCharacterWakeUp()
        {
            DivaEntity diva = Container.Instance.FindEntity<DivaEntity>();
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
            return _animationAnalytic.GetAnimationMode() is EDivaAnimationMode.Sleep;
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

        private void _onEnteredMode(EDivaAnimationMode currentMode)
        {
            if (currentMode is not EDivaAnimationMode.Sleep)
            {
                _subscribeToEvents(false);
              
                Return(true);
            }
        }
    }
}