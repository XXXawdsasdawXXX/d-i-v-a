using Code.Components.Character;
using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Services;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Stand : BehaviourNode, IBehaviourCallback
    {
        private readonly Character _character;
        private readonly TimeObserver _timeObserver;
        private readonly CharacterLiveStatesAnalytics _characterLiveStateAnalytics;

        private BehaviourNode_RandomSequence _randomSequence;

        public BehaviorNode_Stand()
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

                Debugging.Instance.Log($"Нода стояния: выбрано ", Debugging.Type.BehaviorTree);
                return;
            }

            Debugging.Instance.Log($"Нода стояния: отказ ", Debugging.Type.BehaviorTree);
            Return(false);
        }

        void IBehaviourCallback.InvokeCallback(BehaviourNode node, bool success)
        {
            Return(true);
        }
    }
}