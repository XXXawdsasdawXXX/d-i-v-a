using UnityEngine;

namespace Code.Infrastructure.BehaviorTree.BaseNodes
{
    public sealed class BehaviourNode_Selector : BehaviourNode, IBehaviourCallback
    {
        [SerializeField]
        private BehaviourNode[] _orderedNodes;
        private BehaviourNode _currentChild;

        private int _currentChildIndex;
        
        protected override void Run()
        {
            if (_orderedNodes == null || _orderedNodes.Length <= 0)
            {
                Return(false);
                return;
            }

            _currentChildIndex = 0;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        void IBehaviourCallback.Invoke(BehaviourNode node, bool success)
        {
            if (success)
            {
                Return(true);
                return;
            }

            if (_currentChildIndex + 1 >= _orderedNodes.Length)
            {
                Return(false);
                return;
            }

            _currentChildIndex++;
            _currentChild = _orderedNodes[_currentChildIndex];
            _currentChild.Run(callback: this);
        }

        protected override void OnBreak()
        {
            if (_currentChild != null && _currentChild.IsRunning)
            {
                _currentChild.Break();
            }
        }

        protected override void OnReturn(bool result)
        {
    //        Debug.Log($"NODE: {name} RETURN: {result}");
        }

        protected override void OnDispose()
        {
            _currentChild = null;
        }
        
    }
}