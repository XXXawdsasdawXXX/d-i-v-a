using Code.Components.Entities.Characters;
using Code.Data.Enums;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;

namespace Code.Infrastructure.BehaviorTree.Hand.Behavior
{
    public class BehaviourNode_WaitCharacterWakeUp : BaseNode
    {
        private readonly CharacterAnimationAnalytic _animationAnalytic;

        public BehaviourNode_WaitCharacterWakeUp()
        {
            DIVA diva = Container.Instance.FindEntity<DIVA>();
            _animationAnalytic = diva.FindCharacterComponent<CharacterAnimationAnalytic>();
        }

        protected override void Run()
        {
            if (IsCanRun())
            {
                SubscribeToEvents(true);
                return;
            }

            Return(false);
        }

        protected override bool IsCanRun()
        {
            return _animationAnalytic.GetAnimationMode() is CharacterAnimationMode.Sleep;
        }

        private void SubscribeToEvents(bool flag)
        {
            if (flag)
            {
                _animationAnalytic.OnEnteredMode += OnEnteredMode;
            }
            else
            {
                _animationAnalytic.OnEnteredMode -= OnEnteredMode;
            }
        }

        private void OnEnteredMode(CharacterAnimationMode currentMode)
        {
            if (currentMode is not CharacterAnimationMode.Sleep)
            {
                SubscribeToEvents(false);
                Return(true);
            }
        }
    }
}