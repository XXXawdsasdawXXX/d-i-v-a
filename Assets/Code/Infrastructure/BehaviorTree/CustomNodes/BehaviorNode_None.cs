using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Services;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_None : BehaviourNode, IBehaviourCallback
    {
        private readonly Character _character;
        private readonly TimeObserver _timeObserver;
        private readonly CharacterLiveStatesAnalytics _characterLiveStateAnalytics;

        private BehaviourNode_RandomSequence _randomSequence;

        public BehaviorNode_None()
        {
            _character = Container.Instance.GetCharacter();

            _timeObserver = Container.Instance.FindService<TimeObserver>();
            _characterLiveStateAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();

            _randomSequence = new BehaviourNode_RandomSequence(new BehaviourNode[]
            {
                new BehaviourNode_WaitForSeconds(new RangedFloat()
                {
                    MinValue = 60 * 1,
                    MaxValue = 60 * 5
                }), 
                new BehaviourNode_LookToMouse()
            });
        }

        protected override void OnBreak()
        {
            _randomSequence.Break();
            base.OnBreak();
        }

        protected override void Run()
        {
            if (_characterLiveStateAnalytics.CurrentLowerLiveStateKey == LiveStateKey.None)
            {
                _character.Animator.EnterToMode(CharacterAnimationMode.Stand);
                _randomSequence.Run(this);
                return;
            }

            Return(false);
        }

        void IBehaviourCallback.Invoke(BehaviourNode node, bool success)
        {
             Return(true);
        }
    }
}