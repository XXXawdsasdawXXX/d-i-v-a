using Code.Components.Character.LiveState;
using Code.Components.Characters;
using Code.Data.Enums;
using Code.Data.Value.RangeFloat;
using Code.Infrastructure.BehaviorTree.BaseNodes;
using Code.Infrastructure.DI;
using Code.Utils;

namespace Code.Infrastructure.BehaviorTree.CustomNodes
{
    public class BehaviorNode_Stand : BaseNode, IBehaviourCallback
    {
        private readonly Character _character;
        private readonly CharacterLiveStatesAnalytics _characterLiveStateAnalytics;
        
        private readonly BaseNode _reactionVoiceNode;
        private readonly BaseNode_RandomSequence _randomSequenceNode;

        public BehaviorNode_Stand()
        {
            _character = Container.Instance.FindEntity<Character>();
            _characterLiveStateAnalytics = Container.Instance.FindLiveStateLogic<CharacterLiveStatesAnalytics>();

            _randomSequenceNode = new BaseNode_RandomSequence(new BaseNode[]
            {
                new SubNode_WaitForSeconds(new RangedFloat()
                {
                    MinValue = 60 * 1,
                    MaxValue = 60 * 5
                }),
                new SubNode_LookToMouse()
            });
        }

        protected override void Run()
        {
            if (_characterLiveStateAnalytics.CurrentLowerLiveStateKey == LiveStateKey.None)
            {
                _character.Animator.EnterToMode(CharacterAnimationMode.Stand);
                _randomSequenceNode.Run(this);

                Debugging.Instance.Log($"Нода стояния: выбрано", Debugging.Type.BehaviorTree);
                
                return;
            }

            Debugging.Instance.Log($"Нода стояния: отказ", Debugging.Type.BehaviorTree);
            Return(false);
        }

        protected override void OnBreak()
        {
            _randomSequenceNode.Break();
            base.OnBreak();
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
            Return(true);
        }
    }
}