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
        private readonly CharacterAnimator _characterAnimator;
        private readonly CharacterLiveStatesAnalytic _statesAnalytic;

        private readonly BaseNode_RandomSequence _randomSequenceNode;

        private BaseNode _currentNode;

        public BehaviorNode_Stand()
        {
            var character = Container.Instance.FindEntity<Character>();

            _statesAnalytic = character.FindCharacterComponent<CharacterLiveStatesAnalytic>();
            _characterAnimator = character.FindCharacterComponent<CharacterAnimator>();

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
            if (IsCanStand())
            {
                Debugging.Instance.Log($"Нода стояния: выбрано", Debugging.Type.BehaviorTree);
                _characterAnimator.EnterToMode(CharacterAnimationMode.Stand);
                RunNode(_randomSequenceNode);
            }
            else
            {
                Debugging.Instance.Log(
                    $"Нода стояния: отказ. текущий минимальный стрейт {_statesAnalytic.CurrentLowerLiveStateKey}",
                    Debugging.Type.BehaviorTree);

                Return(false);
            }
        }

        void IBehaviourCallback.InvokeCallback(BaseNode node, bool success)
        {
                Debugging.Instance.Log($"Нода стояния: колбэк ", Debugging.Type.BehaviorTree);
            if (_statesAnalytic.CurrentLowerLiveStateKey == LiveStateKey.None)
            {
                Debugging.Instance.Log($"Нода стояния: колбэк -> запуск рандомной сиквенции",
                    Debugging.Type.BehaviorTree);
                RunNode(_randomSequenceNode);
            }
            else
            {
                Debugging.Instance.Log($"Нода стояния: колбэк -> ретерн тру", Debugging.Type.BehaviorTree);
                Return(true);
            }
        }

        protected override void OnBreak()
        {
            Debugging.Instance.Log($"Нода стояния: брейк ", Debugging.Type.BehaviorTree);

            _currentNode?.Break();
            base.OnBreak();
        }

        private bool IsCanStand()
        {
            return true;
        }


        private void RunNode(BaseNode node)
        {
            if (_currentNode != null && _currentNode == node)
            {
                return;
            }

            _currentNode?.Break();
            _currentNode = node;
            _currentNode.Run(this);
        }
    }
}